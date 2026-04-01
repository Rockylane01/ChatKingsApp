using ChatKingsApp.Data;
using ChatKingsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatKingsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PredictionsController : ControllerBase
{
    private readonly ChatKingsDbContext _context;

    public PredictionsController(ChatKingsDbContext context)
    {
        _context = context;
    }

    // ── DTOs ────────────────────────────────────────────────────────────

    public sealed class CreatePredictionRequest
    {
        public int chat_id { get; set; }
        public int user_id { get; set; }
        public string title { get; set; } = null!;
        public string? espn_event_id { get; set; }
        public DateTime? lock_at { get; set; }
        public int initial_bet_min { get; set; } = 20;
        public int initial_bet_max { get; set; } = 100;
        public List<CreateOptionDto> options { get; set; } = [];
        public CreatorWagerDto? creator_wager { get; set; }
    }

    public sealed class CreateOptionDto
    {
        public string option_label { get; set; } = null!;
        public int? team_id { get; set; }
        public int display_order { get; set; }
    }

    public sealed class CreatorWagerDto
    {
        public int option_index { get; set; }
        public int points_wagered { get; set; }
    }

    public sealed class PlaceWagerRequest
    {
        public int user_id { get; set; }
        public int option_id { get; set; }
        public int points_wagered { get; set; }
    }

    public sealed class ResolveRequest
    {
        public int winning_option_id { get; set; }
        public int resolved_by_user_id { get; set; }
    }

    // ── POST /api/predictions ───────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> CreatePrediction([FromBody] CreatePredictionRequest req)
    {
        // Validate options count
        if (req.options is not { Count: 2 })
            return BadRequest("Exactly 2 options are required.");

        if (req.initial_bet_min < 20)
            return BadRequest("initial_bet_min must be >= 20.");

        if (req.initial_bet_max < req.initial_bet_min)
            return BadRequest("initial_bet_max must be >= initial_bet_min.");

        if (req.creator_wager is null)
            return BadRequest("Creator must place an initial wager.");

        if (req.creator_wager.option_index is < 0 or > 1)
            return BadRequest("creator_wager.option_index must be 0 or 1.");

        if (req.creator_wager.points_wagered < req.initial_bet_min ||
            req.creator_wager.points_wagered > req.initial_bet_max)
            return BadRequest($"Creator wager must be between {req.initial_bet_min} and {req.initial_bet_max}.");

        // Look up creator membership
        var member = await _context.ChatMembers
            .FirstOrDefaultAsync(m => m.user_id == req.user_id && m.chat_id == req.chat_id && m.is_active);

        if (member is null)
            return BadRequest("User is not an active member of this chat.");

        if (member.points_balance < req.creator_wager.points_wagered)
            return BadRequest("Insufficient points balance.");

        // ── King didn't wager on previous prediction? Penalize 10 pts ──
        if (member.is_king)
        {
            var lastPrediction = await _context.Predictions
                .Where(p => p.chat_id == req.chat_id && p.status != "cancelled")
                .OrderByDescending(p => p.created_at)
                .FirstOrDefaultAsync();

            if (lastPrediction is not null)
            {
                bool kingWagered = await _context.Wagers
                    .AnyAsync(w => w.prediction_id == lastPrediction.prediction_id && w.user_id == req.user_id);

                if (!kingWagered)
                {
                    int penalty = Math.Min(10, member.points_balance);
                    member.points_balance -= penalty;

                    if (penalty > 0)
                    {
                        _context.PointsLedgers.Add(new PointsLedger
                        {
                            user_id = req.user_id,
                            chat_id = req.chat_id,
                            prediction_id = lastPrediction.prediction_id,
                            change_amount = -penalty,
                            change_reason = "king_missed_wager_penalty",
                            created_at = DateTime.UtcNow
                        });
                    }

                    // Re-check balance after penalty
                    if (member.points_balance < req.creator_wager.points_wagered)
                        return BadRequest("Insufficient points balance after King penalty.");
                }
            }
        }

        // Build prediction
        var prediction = new Prediction
        {
            chat_id = req.chat_id,
            created_by_user_id = req.user_id,
            game_id = 0,
            title = req.title,
            prediction_type = "Basketball:WinLoss",
            status = "pending",
            pot_points = req.creator_wager.points_wagered,
            created_at = DateTime.UtcNow,
            lock_at = req.lock_at,
            espn_event_id = req.espn_event_id,
            initial_bet_min = req.initial_bet_min,
            initial_bet_max = req.initial_bet_max
        };

        foreach (var o in req.options)
        {
            prediction.Options.Add(new PredictionOption
            {
                option_label = o.option_label,
                team_id = o.team_id,
                display_order = o.display_order
            });
        }

        _context.Predictions.Add(prediction);
        await _context.SaveChangesAsync(); // generates prediction_id + option_ids

        // Place creator wager
        var chosenOption = prediction.Options
            .OrderBy(o => o.display_order)
            .ElementAt(req.creator_wager.option_index);

        var wager = new Wager
        {
            prediction_id = prediction.prediction_id,
            option_id = chosenOption.option_id,
            user_id = req.user_id,
            chat_id = req.chat_id,
            points_wagered = req.creator_wager.points_wagered,
            result_status = "pending",
            placed_at = DateTime.UtcNow
        };

        _context.Wagers.Add(wager);

        // Deduct points
        member.points_balance -= req.creator_wager.points_wagered;

        _context.PointsLedgers.Add(new PointsLedger
        {
            user_id = req.user_id,
            chat_id = req.chat_id,
            prediction_id = prediction.prediction_id,
            wager_id = null, // wager_id not yet saved
            change_amount = -req.creator_wager.points_wagered,
            change_reason = "wager_placed",
            created_at = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        // Update ledger with wager_id now that it's saved
        var ledgerEntry = await _context.PointsLedgers
            .Where(l => l.prediction_id == prediction.prediction_id
                        && l.user_id == req.user_id
                        && l.change_reason == "wager_placed"
                        && l.wager_id == null)
            .OrderByDescending(l => l.created_at)
            .FirstOrDefaultAsync();

        if (ledgerEntry is not null)
        {
            ledgerEntry.wager_id = wager.wager_id;
            await _context.SaveChangesAsync();
        }

        // Return created prediction with options as DTO to avoid circular references
        var saved = await _context.Predictions
            .Include(p => p.Options)
            .FirstAsync(p => p.prediction_id == prediction.prediction_id);

        var dto = new
        {
            saved.prediction_id,
            saved.chat_id,
            saved.created_by_user_id,
            saved.game_id,
            saved.title,
            saved.description,
            saved.prediction_type,
            saved.status,
            saved.pot_points,
            saved.created_at,
            saved.lock_at,
            saved.resolved_at,
            saved.espn_event_id,
            saved.initial_bet_min,
            saved.initial_bet_max,
            options = saved.Options.Select(o => new
            {
                o.option_id,
                o.prediction_id,
                o.option_label,
                o.team_id,
                o.display_order
            })
        };

        return CreatedAtAction(nameof(GetPrediction), new { id = dto.prediction_id }, dto);
    }

    // ── GET /api/predictions?chatId={id}&status={status} ────────────────

    [HttpGet]
    public async Task<IActionResult> ListPredictions([FromQuery] int? chatId, [FromQuery] string? status)
    {
        var query = _context.Predictions
            .Include(p => p.Options)
            .AsQueryable();

        if (chatId.HasValue)
            query = query.Where(p => p.chat_id == chatId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(p => p.status == status);

        var predictions = await query
            .OrderByDescending(p => p.created_at)
            .ToListAsync();

        var predictionIds = predictions.Select(p => p.prediction_id).ToList();

        // Get wager aggregates per option
        var wagerStats = await _context.Wagers
            .Where(w => predictionIds.Contains(w.prediction_id))
            .GroupBy(w => new { w.prediction_id, w.option_id })
            .Select(g => new
            {
                g.Key.prediction_id,
                g.Key.option_id,
                wager_count = g.Count(),
                total_points = g.Sum(w => w.points_wagered)
            })
            .ToListAsync();

        var result = predictions.Select(p => new
        {
            p.prediction_id,
            p.chat_id,
            p.created_by_user_id,
            p.game_id,
            p.title,
            p.description,
            p.prediction_type,
            p.status,
            p.pot_points,
            p.created_at,
            p.lock_at,
            p.resolved_at,
            p.espn_event_id,
            p.initial_bet_min,
            p.initial_bet_max,
            options = p.Options.Select(o => new
            {
                o.option_id,
                o.prediction_id,
                o.option_label,
                o.team_id,
                o.display_order,
                wager_count = wagerStats
                    .Where(s => s.prediction_id == p.prediction_id && s.option_id == o.option_id)
                    .Select(s => s.wager_count)
                    .FirstOrDefault(),
                total_points = wagerStats
                    .Where(s => s.prediction_id == p.prediction_id && s.option_id == o.option_id)
                    .Select(s => s.total_points)
                    .FirstOrDefault()
            })
        });

        return Ok(result);
    }

    // ── GET /api/predictions/{id} ───────────────────────────────────────

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPrediction(int id)
    {
        var prediction = await _context.Predictions
            .Include(p => p.Options)
            .ThenInclude(o => o.Wagers)
            .FirstOrDefaultAsync(p => p.prediction_id == id);

        if (prediction is null)
            return NotFound();

        var resolution = await _context.PredictionResolutions
            .FirstOrDefaultAsync(r => r.prediction_id == id);

        var result = new
        {
            prediction.prediction_id,
            prediction.chat_id,
            prediction.created_by_user_id,
            prediction.game_id,
            prediction.title,
            prediction.description,
            prediction.prediction_type,
            prediction.status,
            prediction.pot_points,
            prediction.created_at,
            prediction.lock_at,
            prediction.resolved_at,
            prediction.espn_event_id,
            prediction.initial_bet_min,
            prediction.initial_bet_max,
            options = prediction.Options.Select(o => new
            {
                o.option_id,
                o.prediction_id,
                o.option_label,
                o.team_id,
                o.display_order,
                wagers = o.Wagers.Select(w => new
                {
                    w.wager_id,
                    w.user_id,
                    w.points_wagered,
                    w.result_status,
                    w.placed_at
                })
            }),
            resolution = resolution is null ? null : new
            {
                resolution.resolution_id,
                resolution.prediction_id,
                resolution.winning_option_id,
                resolution.resolved_by_user_id,
                resolution.notes,
                resolution.resolved_at
            }
        };

        return Ok(result);
    }

    // ── POST /api/predictions/{id}/wager ────────────────────────────────

    [HttpPost("{id}/wager")]
    public async Task<IActionResult> PlaceWager(int id, [FromBody] PlaceWagerRequest req)
    {
        var prediction = await _context.Predictions
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.prediction_id == id);

        if (prediction is null)
            return NotFound("Prediction not found.");

        if (prediction.status != "pending")
            return BadRequest("Prediction is not open for wagers.");

        if (prediction.lock_at.HasValue && DateTime.UtcNow >= prediction.lock_at.Value)
            return BadRequest("Prediction is locked.");

        if (req.points_wagered < prediction.initial_bet_min || req.points_wagered > prediction.initial_bet_max)
            return BadRequest($"Wager must be between {prediction.initial_bet_min} and {prediction.initial_bet_max}.");

        if (!prediction.Options.Any(o => o.option_id == req.option_id))
            return BadRequest("Invalid option_id for this prediction.");

        // Check for existing wager
        bool alreadyWagered = await _context.Wagers
            .AnyAsync(w => w.prediction_id == id && w.user_id == req.user_id);

        if (alreadyWagered)
            return BadRequest("User already has a wager on this prediction.");

        // Check daily strikes
        var todayUtc = DateTime.UtcNow.Date;
        int strikesToday = await _context.StrikeEvents
            .CountAsync(s => s.user_id == req.user_id
                             && s.chat_id == prediction.chat_id
                             && s.created_at >= todayUtc
                             && s.created_at < todayUtc.AddDays(1));

        if (strikesToday >= 3)
            return BadRequest("User has reached the daily strike limit (3). Cannot place wager.");

        // Check points balance
        var member = await _context.ChatMembers
            .FirstOrDefaultAsync(m => m.user_id == req.user_id && m.chat_id == prediction.chat_id && m.is_active);

        if (member is null)
            return BadRequest("User is not an active member of this chat.");

        if (member.points_balance < req.points_wagered)
            return BadRequest("Insufficient points balance.");

        // Create wager
        var wager = new Wager
        {
            prediction_id = id,
            option_id = req.option_id,
            user_id = req.user_id,
            chat_id = prediction.chat_id,
            points_wagered = req.points_wagered,
            result_status = "pending",
            placed_at = DateTime.UtcNow
        };

        _context.Wagers.Add(wager);

        // Deduct points
        member.points_balance -= req.points_wagered;

        // Update pot
        prediction.pot_points += req.points_wagered;

        await _context.SaveChangesAsync();

        // Log in PointsLedger (after save so wager_id is available)
        _context.PointsLedgers.Add(new PointsLedger
        {
            user_id = req.user_id,
            chat_id = prediction.chat_id,
            prediction_id = id,
            wager_id = wager.wager_id,
            change_amount = -req.points_wagered,
            change_reason = "wager_placed",
            created_at = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            wager.wager_id,
            wager.prediction_id,
            wager.option_id,
            wager.user_id,
            wager.chat_id,
            wager.points_wagered,
            wager.result_status,
            wager.placed_at,
            wager.resolved_at
        });
    }

    // ── POST /api/predictions/{id}/resolve ──────────────────────────────

    [HttpPost("{id}/resolve")]
    public async Task<IActionResult> ResolvePrediction(int id, [FromBody] ResolveRequest req)
    {
        var prediction = await _context.Predictions
            .Include(p => p.Options)
            .Include(p => p.Wagers)
            .FirstOrDefaultAsync(p => p.prediction_id == id);

        if (prediction is null)
            return NotFound("Prediction not found.");

        if (prediction.status == "resolved")
            return BadRequest("Prediction is already resolved.");

        if (prediction.status == "cancelled")
            return BadRequest("Prediction is cancelled.");

        if (!prediction.Options.Any(o => o.option_id == req.winning_option_id))
            return BadRequest("Invalid winning_option_id.");

        var now = DateTime.UtcNow;

        // 1. Update prediction status
        prediction.status = "resolved";
        prediction.resolved_at = now;

        // 2. Create resolution record
        var resolution = new PredictionResolution
        {
            prediction_id = id,
            winning_option_id = req.winning_option_id,
            resolved_by_user_id = req.resolved_by_user_id,
            resolved_at = now
        };
        _context.PredictionResolutions.Add(resolution);

        // 3. Calculate pot and payouts
        var allWagers = prediction.Wagers.ToList();
        int totalPot = allWagers.Sum(w => w.points_wagered);

        var winningWagers = allWagers.Where(w => w.option_id == req.winning_option_id).ToList();
        var losingWagers = allWagers.Where(w => w.option_id != req.winning_option_id).ToList();

        int winningSideTotal = winningWagers.Sum(w => w.points_wagered);
        int losingSideTotal = losingWagers.Sum(w => w.points_wagered);

        bool oneSidedBet = losingSideTotal == 0 && winningWagers.Count > 0;

        if (oneSidedBet)
        {
            // 4. Only one side bet: add 20 to pot, redistribute proportionally
            totalPot += 20;
        }

        // 5. Determine if minority bonus applies
        bool minorityBonus = !oneSidedBet && winningSideTotal > 0 && winningSideTotal < losingSideTotal;

        // Load chat members we will need to update
        var userIds = allWagers.Select(w => w.user_id).Distinct().ToList();
        var members = await _context.ChatMembers
            .Where(m => m.chat_id == prediction.chat_id && userIds.Contains(m.user_id) && m.is_active)
            .ToListAsync();

        var memberLookup = members.ToDictionary(m => m.user_id);

        // 7. Update wager statuses and credit winners
        foreach (var w in winningWagers)
        {
            w.result_status = "won";
            w.resolved_at = now;

            // Proportional payout
            decimal payout;
            if (winningSideTotal > 0)
                payout = (decimal)w.points_wagered / winningSideTotal * totalPot;
            else
                payout = 0;

            // 6. Minority bonus
            if (minorityBonus)
                payout *= 1.20m;

            int payoutInt = (int)Math.Floor(payout);
            if (payoutInt < 0) payoutInt = 0;

            // 8. Credit winner
            if (memberLookup.TryGetValue(w.user_id, out var winnerMember))
            {
                winnerMember.points_balance += payoutInt;

                _context.PointsLedgers.Add(new PointsLedger
                {
                    user_id = w.user_id,
                    chat_id = prediction.chat_id,
                    prediction_id = id,
                    wager_id = w.wager_id,
                    change_amount = payoutInt,
                    change_reason = "wager_won",
                    created_at = now
                });
            }
        }

        // 9. Strike losers
        foreach (var w in losingWagers)
        {
            w.result_status = "lost";
            w.resolved_at = now;

            _context.StrikeEvents.Add(new StrikeEvent
            {
                user_id = w.user_id,
                chat_id = prediction.chat_id,
                reason = "lost_wager",
                strike_value = 1,
                created_at = now
            });
        }

        // Also mark wagers on losing side with no opposition as won (one-sided case)
        // (Already handled above — winningWagers covers this scenario.)

        await _context.SaveChangesAsync();

        // 10. Check crown transfer: does any member now have more points than current King?
        var chatMembers = await _context.ChatMembers
            .Where(m => m.chat_id == prediction.chat_id && m.is_active)
            .ToListAsync();

        var currentKing = chatMembers.FirstOrDefault(m => m.is_king);
        var topMember = chatMembers.OrderByDescending(m => m.points_balance).FirstOrDefault();

        if (topMember is not null &&
            (currentKing is null || (topMember.user_id != currentKing.user_id && topMember.points_balance > currentKing.points_balance)))
        {
            // Clear all is_king flags, then set new King
            foreach (var cm in chatMembers)
                cm.is_king = false;
            topMember.is_king = true;

            // Also sync Chat.chat_king_user_id
            var chat = await _context.Chats.FindAsync(prediction.chat_id);
            if (chat is not null)
            {
                chat.chat_king_user_id = topMember.user_id;
                chat.updated_at = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        // 11. Points can never go below 0 — clamp
        var allChatMembers = await _context.ChatMembers
            .Where(m => m.chat_id == prediction.chat_id && m.points_balance < 0)
            .ToListAsync();

        foreach (var m in allChatMembers)
            m.points_balance = 0;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            prediction.prediction_id,
            prediction.status,
            prediction.resolved_at,
            resolution = new
            {
                resolution.resolution_id,
                resolution.winning_option_id,
                resolution.resolved_by_user_id,
                resolution.resolved_at
            },
            winners = winningWagers.Select(w => new
            {
                w.wager_id,
                w.user_id,
                w.points_wagered,
                w.result_status
            }),
            losers = losingWagers.Select(w => new
            {
                w.wager_id,
                w.user_id,
                w.points_wagered,
                w.result_status
            })
        });
    }
}
