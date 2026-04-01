using System.Text.Json;
using ChatKingsApp.Data;
using ChatKingsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatKingsApp.Services;

public class GameResolutionService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GameResolutionService> _logger;

    private const string EspnScoreboardUrl =
        "https://site.api.espn.com/apis/site/v2/sports/basketball/mens-college-basketball/scoreboard";

    public GameResolutionService(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<GameResolutionService> logger)
    {
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("GameResolutionService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PollAndResolveAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GameResolutionService polling loop.");
            }

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    private async Task PollAndResolveAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ChatKingsDbContext>();

        var pendingPredictions = await db.Predictions
            .Include(p => p.Options)
            .Include(p => p.Wagers)
            .Where(p => p.status == "pending"
                        && p.espn_event_id != null
                        && p.lock_at < DateTime.UtcNow)
            .ToListAsync(ct);

        if (pendingPredictions.Count == 0)
            return;

        var distinctEventIds = pendingPredictions
            .Select(p => p.espn_event_id!)
            .Distinct()
            .ToList();

        // Fetch ESPN scoreboard
        var client = _httpClientFactory.CreateClient();
        JsonElement eventsArray;

        try
        {
            var json = await client.GetStringAsync(EspnScoreboardUrl, ct);
            var doc = JsonDocument.Parse(json);
            eventsArray = doc.RootElement.GetProperty("events");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch ESPN scoreboard.");
            return;
        }

        // Build a lookup of completed events: eventId -> winning team displayName
        var completedEvents = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var evt in eventsArray.EnumerateArray())
        {
            var eventId = evt.GetProperty("id").GetString();
            if (eventId == null || !distinctEventIds.Contains(eventId))
                continue;

            var state = evt.GetProperty("status")
                          .GetProperty("type")
                          .GetProperty("state")
                          .GetString();

            if (state != "post")
                continue;

            var competitors = evt.GetProperty("competitions")[0]
                                 .GetProperty("competitors");

            foreach (var comp in competitors.EnumerateArray())
            {
                if (comp.TryGetProperty("winner", out var winnerProp) && winnerProp.GetBoolean())
                {
                    var displayName = comp.GetProperty("team")
                                         .GetProperty("displayName")
                                         .GetString();
                    if (displayName != null)
                        completedEvents[eventId] = displayName;
                    break;
                }
            }
        }

        // Resolve matching predictions
        foreach (var prediction in pendingPredictions)
        {
            if (!completedEvents.TryGetValue(prediction.espn_event_id!, out var winningTeamName))
                continue;

            var winningOption = prediction.Options.FirstOrDefault(o =>
                o.option_label.Contains(winningTeamName, StringComparison.OrdinalIgnoreCase));

            if (winningOption == null)
            {
                _logger.LogWarning(
                    "Prediction {PredictionId}: no option matches winning team '{Team}'.",
                    prediction.prediction_id, winningTeamName);
                continue;
            }

            try
            {
                await ResolvePredictionAsync(db, prediction, winningOption, ct);
                _logger.LogInformation(
                    "Resolved prediction {PredictionId} — winner: {Team}.",
                    prediction.prediction_id, winningTeamName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to resolve prediction {PredictionId}.", prediction.prediction_id);
            }
        }
    }

    private static async Task ResolvePredictionAsync(
        ChatKingsDbContext db,
        Prediction prediction,
        PredictionOption winningOption,
        CancellationToken ct)
    {
        // 1. Mark prediction as resolved
        prediction.status = "resolved";
        prediction.resolved_at = DateTime.UtcNow;

        // 2. Create PredictionResolution
        db.PredictionResolutions.Add(new PredictionResolution
        {
            prediction_id = prediction.prediction_id,
            winning_option_id = winningOption.option_id,
            resolved_by_user_id = 0, // system
            resolved_at = DateTime.UtcNow,
        });

        // 3. Load all wagers for this prediction (they should already be loaded, but ensure)
        var wagers = await db.Wagers
            .Where(w => w.prediction_id == prediction.prediction_id)
            .ToListAsync(ct);

        if (wagers.Count == 0)
        {
            await db.SaveChangesAsync(ct);
            return;
        }

        // 4. Compute pot
        var totalPot = wagers.Sum(w => (long)w.points_wagered);

        var winningWagers = wagers.Where(w => w.option_id == winningOption.option_id).ToList();
        var losingWagers = wagers.Where(w => w.option_id != winningOption.option_id).ToList();

        var totalWinningWagered = winningWagers.Sum(w => (long)w.points_wagered);
        var totalLosingWagered = losingWagers.Sum(w => (long)w.points_wagered);

        // 6. One-sided: if no losing wagers, add 20 to pot
        if (losingWagers.Count == 0)
            totalPot += 20;

        // 8. Minority bonus: if winners wagered less than losers
        bool minorityBonus = losingWagers.Count > 0 && totalWinningWagered < totalLosingWagered;

        // Distribute payouts to winners
        foreach (var wager in winningWagers)
        {
            double share = totalWinningWagered > 0
                ? (double)wager.points_wagered / totalWinningWagered
                : 0;
            double payout = share * totalPot;

            if (minorityBonus)
                payout *= 1.2;

            int payoutInt = (int)Math.Round(payout);

            wager.result_status = "won";
            wager.resolved_at = DateTime.UtcNow;

            // Credit winner
            var member = await db.ChatMembers
                .FirstOrDefaultAsync(m => m.user_id == wager.user_id
                                          && m.chat_id == wager.chat_id
                                          && m.is_active, ct);
            if (member != null)
            {
                member.points_balance += payoutInt;

                var changeReason = minorityBonus
                    ? "prediction_payout_minority_bonus"
                    : "prediction_payout";

                db.PointsLedgers.Add(new PointsLedger
                {
                    user_id = wager.user_id,
                    chat_id = wager.chat_id,
                    prediction_id = prediction.prediction_id,
                    wager_id = wager.wager_id,
                    change_amount = payoutInt,
                    change_reason = changeReason,
                    created_at = DateTime.UtcNow,
                });
            }
        }

        // Mark losing wagers and create strikes
        foreach (var wager in losingWagers)
        {
            wager.result_status = "lost";
            wager.resolved_at = DateTime.UtcNow;

            db.StrikeEvents.Add(new StrikeEvent
            {
                user_id = wager.user_id,
                chat_id = wager.chat_id,
                reason = "lost_wager",
                strike_value = 1,
                created_at = DateTime.UtcNow,
            });
        }

        // 12. Crown check: find member with highest points_balance in the chat
        var chatId = prediction.chat_id;
        var members = await db.ChatMembers
            .Where(m => m.chat_id == chatId && m.is_active)
            .ToListAsync(ct);

        if (members.Count > 0)
        {
            var topMember = members.OrderByDescending(m => m.points_balance).First();
            var currentKing = members.FirstOrDefault(m => m.is_king);

            if (currentKing == null ||
                (currentKing.chat_member_id != topMember.chat_member_id && topMember.points_balance > currentKing.points_balance))
            {
                foreach (var m in members)
                    m.is_king = false;
                topMember.is_king = true;

                // Sync Chat.chat_king_user_id
                var chat = await db.Chats.FindAsync(new object[] { chatId }, ct);
                if (chat != null)
                {
                    chat.chat_king_user_id = topMember.user_id;
                    chat.updated_at = DateTime.UtcNow;
                }
            }
        }

        // 13. Save
        await db.SaveChangesAsync(ct);
    }
}
