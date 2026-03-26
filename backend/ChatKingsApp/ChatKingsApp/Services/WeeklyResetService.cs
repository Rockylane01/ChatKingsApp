using ChatKingsApp.Data;
using ChatKingsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatKingsApp.Services;

public class WeeklyResetService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WeeklyResetService> _logger;

    public WeeklyResetService(
        IServiceScopeFactory scopeFactory,
        ILogger<WeeklyResetService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WeeklyResetService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndResetAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in WeeklyResetService polling loop.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task CheckAndResetAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ChatKingsDbContext>();

        var chats = await db.Chats.ToListAsync(ct);

        foreach (var chat in chats)
        {
            try
            {
                await TryResetChatAsync(db, chat, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed weekly reset for chat {ChatId}.", chat.chat_id);
            }
        }
    }

    private async Task TryResetChatAsync(ChatKingsDbContext db, Chat chat, CancellationToken ct)
    {
        TimeZoneInfo tz;
        try
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById(chat.timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            _logger.LogWarning(
                "Chat {ChatId} has unknown timezone '{Timezone}', skipping reset.",
                chat.chat_id, chat.timezone);
            return;
        }

        var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

        // Must be Sunday and past midnight
        if (localNow.DayOfWeek != DayOfWeek.Sunday)
            return;

        var today = DateOnly.FromDateTime(localNow);

        // Check if we already created a snapshot for this chat today (prevents double-reset)
        bool alreadyReset = await db.ChatLeaderboardSnapshots
            .AnyAsync(s => s.chat_id == chat.chat_id && s.snapshot_date == today, ct);

        if (alreadyReset)
            return;

        // Load active members
        var members = await db.ChatMembers
            .Where(m => m.chat_id == chat.chat_id && m.is_active)
            .ToListAsync(ct);

        if (members.Count == 0)
            return;

        // b. Create leaderboard snapshots ranked by points_balance desc
        var ranked = members.OrderByDescending(m => m.points_balance).ToList();
        for (int i = 0; i < ranked.Count; i++)
        {
            db.ChatLeaderboardSnapshots.Add(new ChatLeaderboardSnapshot
            {
                chat_id = chat.chat_id,
                user_id = ranked[i].user_id,
                rank_position = i + 1,
                points_balance = ranked[i].points_balance,
                snapshot_date = today,
            });
        }

        // c. Crown new King: member with highest points_balance.
        //    Ties: prior king keeps crown.
        var currentKing = members.FirstOrDefault(m => m.is_king);
        var topBalance = ranked[0].points_balance;
        var topMembers = ranked.Where(m => m.points_balance == topBalance).ToList();

        ChatMember newKing;
        if (currentKing != null && topMembers.Any(m => m.chat_member_id == currentKing.chat_member_id))
        {
            // Current king is tied for first — they keep the crown
            newKing = currentKing;
        }
        else
        {
            newKing = topMembers[0];
        }

        // e. Clear is_king on all except new King
        foreach (var m in members)
            m.is_king = false;
        newKing.is_king = true;

        // d. Reset all members' points_balance to 1000 and log in PointsLedger
        foreach (var member in members)
        {
            int oldBalance = member.points_balance;
            int changeAmount = 1000 - oldBalance;

            db.PointsLedgers.Add(new PointsLedger
            {
                user_id = member.user_id,
                chat_id = chat.chat_id,
                prediction_id = null,
                wager_id = null,
                change_amount = changeAmount,
                change_reason = "weekly_reset",
                created_at = DateTime.UtcNow,
            });

            member.points_balance = 1000;
        }

        await db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Weekly reset completed for chat {ChatId} ({ChatName}). {MemberCount} members reset. King: user {KingUserId}.",
            chat.chat_id, chat.chat_name, members.Count, newKing.user_id);
    }
}
