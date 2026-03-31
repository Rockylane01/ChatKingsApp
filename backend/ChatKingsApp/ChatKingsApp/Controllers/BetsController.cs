using System.Text.Json;
using ChatKingsApp.Data;
using ChatKingsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatKingsApp.Controllers;

/// <summary>
/// Bets API for frontend compatibility. Maps to Predictions + Wagers (ERD v2).
/// Each "bet" = 1 Prediction + 1 PredictionOption + 1 Wager.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BetsController : ControllerBase
{
    private static readonly HashSet<string> ValidStatuses = ["pending", "won", "lost", "cancelled"];
    private static readonly HashSet<string> ValidSports = ["Basketball", "Football"];
    private static readonly HashSet<string> ValidCategories = ["Points", "Stats"];

    private readonly ChatKingsDbContext _context;

    public BetsController(ChatKingsDbContext context)
    {
        _context = context;
    }

    private static string? ValidatePredictionPayload(string betCategory, string predictionDetailsJson)
    {
        if (string.IsNullOrWhiteSpace(betCategory))
            return "bet_category is required.";

        var parts = betCategory.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            return "bet_category must be in format 'Sport:Category' (e.g. Basketball:Points).";

        if (!ValidSports.Contains(parts[0]))
            return $"Sport must be one of: {string.Join(", ", ValidSports)}.";
        if (!ValidCategories.Contains(parts[1]))
            return $"Category must be one of: {string.Join(", ", ValidCategories)}.";

        if (string.IsNullOrWhiteSpace(predictionDetailsJson))
            return "prediction_details_json is required.";

        try
        {
            var doc = JsonDocument.Parse(predictionDetailsJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("text", out var text) || string.IsNullOrWhiteSpace(text.GetString()))
                return "prediction_details_json must include a non-empty 'text' field.";
            if (!root.TryGetProperty("dueBy", out var dueBy) || string.IsNullOrWhiteSpace(dueBy.GetString()))
                return "prediction_details_json must include a 'dueBy' field (ISO datetime).";
            if (!root.TryGetProperty("minPoints", out _) || !root.TryGetProperty("maxPoints", out _))
                return "prediction_details_json must include 'minPoints' and 'maxPoints' numbers.";
        }
        catch (JsonException)
        {
            return "prediction_details_json must be valid JSON.";
        }

        return null;
    }

    // POST api/bets
    [HttpPost]
    public async Task<ActionResult<object>> CreateBet([FromBody] CreateBetRequest req)
    {
        if (!ValidStatuses.Contains(req.status))
            return BadRequest($"Invalid status '{req.status}'. Must be one of: {string.Join(", ", ValidStatuses)}.");

        // Only the Chat King can create predictions
        var chat = await _context.Chats.FindAsync(req.chat_id);
        if (chat is null)
            return BadRequest("Chat not found.");
        if (chat.chat_king_user_id != req.user_id)
            return StatusCode(403, "Only the Chat King can create predictions.");

        var validationError = ValidatePredictionPayload(req.bet_category, req.prediction_details_json);
        if (validationError != null)
            return BadRequest(validationError);

        var details = JsonDocument.Parse(req.prediction_details_json).RootElement;
        var text = details.GetProperty("text").GetString() ?? "";
        var dueByStr = details.GetProperty("dueBy").GetString() ?? "";
        var minPoints = details.TryGetProperty("minPoints", out var mp) ? mp.GetInt32() : 10;
        var maxPoints = details.TryGetProperty("maxPoints", out var mxp) ? mxp.GetInt32() : 100;
        var lockAt = DateTime.TryParse(dueByStr, out var dt) ? dt : (DateTime?)null;

        var prediction = new Prediction
        {
            chat_id = req.chat_id,
            created_by_user_id = req.user_id,
            game_id = req.game_id,
            title = text,
            description = text,
            prediction_type = req.bet_category,
            status = req.status,
            pot_points = maxPoints,
            created_at = DateTime.UtcNow,
            lock_at = lockAt,
        };
        _context.Predictions.Add(prediction);
        await _context.SaveChangesAsync();

        var option = new PredictionOption
        {
            prediction_id = prediction.prediction_id,
            option_label = text,
            display_order = minPoints,
        };
        _context.PredictionOptions.Add(option);
        await _context.SaveChangesAsync();

        var wager = new Wager
        {
            prediction_id = prediction.prediction_id,
            option_id = option.option_id,
            user_id = req.user_id,
            chat_id = req.chat_id,
            points_wagered = req.points_wagered,
            result_status = req.status,
            placed_at = DateTime.UtcNow,
        };
        _context.Wagers.Add(wager);
        await _context.SaveChangesAsync();

        // Fix prediction_details_json to include actual min/max
        var dto = new
        {
            bet_id = wager.wager_id,
            wager.chat_id,
            game_id = prediction.game_id,
            user_id = wager.user_id,
            bet_category = prediction.prediction_type,
            prediction_details_json = JsonSerializer.Serialize(new { text, dueBy = dueByStr, minPoints, maxPoints }),
            points_wagered = wager.points_wagered,
            status = wager.result_status,
            placed_at = wager.placed_at,
            resolved_at = wager.resolved_at,
        };
        return CreatedAtAction(nameof(GetBet), new { id = wager.wager_id }, dto);
    }

    // PUT api/bets/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<object>> UpdateBet(int id, [FromBody] CreateBetRequest req)
    {
        if (!ValidStatuses.Contains(req.status))
            return BadRequest($"Invalid status '{req.status}'. Must be one of: {string.Join(", ", ValidStatuses)}.");

        var validationError = ValidatePredictionPayload(req.bet_category, req.prediction_details_json);
        if (validationError != null)
            return BadRequest(validationError);

        var wager = await _context.Wagers
            .Include(w => w.Prediction)
            .Include(w => w.PredictionOption)
            .FirstOrDefaultAsync(w => w.wager_id == id);
        if (wager is null)
            return NotFound();

        var details = JsonDocument.Parse(req.prediction_details_json).RootElement;
        var text = details.GetProperty("text").GetString() ?? "";
        var dueByStr = details.GetProperty("dueBy").GetString() ?? "";
        var minPoints = details.TryGetProperty("minPoints", out var mp) ? mp.GetInt32() : 10;
        var maxPoints = details.TryGetProperty("maxPoints", out var mxp) ? mxp.GetInt32() : 100;
        var lockAt = DateTime.TryParse(dueByStr, out var dt) ? dt : (DateTime?)null;

        wager.Prediction.title = text;
        wager.Prediction.description = text;
        wager.Prediction.prediction_type = req.bet_category;
        wager.Prediction.pot_points = maxPoints;
        wager.Prediction.lock_at = lockAt;
        wager.PredictionOption.option_label = text;
        wager.PredictionOption.display_order = minPoints;
        wager.points_wagered = req.points_wagered;
        wager.result_status = req.status;
        wager.resolved_at = req.status == "pending" ? null : DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var dto = new
        {
            bet_id = wager.wager_id,
            wager.chat_id,
            game_id = wager.Prediction.game_id,
            user_id = wager.user_id,
            bet_category = wager.Prediction.prediction_type,
            prediction_details_json = JsonSerializer.Serialize(new { text, dueBy = dueByStr, minPoints, maxPoints }),
            points_wagered = wager.points_wagered,
            status = wager.result_status,
            placed_at = wager.placed_at,
            resolved_at = wager.resolved_at,
        };
        return Ok(dto);
    }

    // DELETE api/bets/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBet(int id)
    {
        var wager = await _context.Wagers
            .Include(w => w.Prediction)
            .FirstOrDefaultAsync(w => w.wager_id == id);
        if (wager is null)
            return NotFound();

        var predId = wager.prediction_id;
        _context.Wagers.Remove(wager);
        var otherWagers = await _context.Wagers.AnyAsync(w => w.prediction_id == predId);
        if (!otherWagers)
        {
            var opts = await _context.PredictionOptions.Where(o => o.prediction_id == predId).ToListAsync();
            _context.PredictionOptions.RemoveRange(opts);
            var pred = await _context.Predictions.FindAsync(predId);
            if (pred != null)
                _context.Predictions.Remove(pred);
        }
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET api/bets
    // GET api/bets?chatId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetBets([FromQuery] int? chatId, [FromQuery] int? gameId)
    {
        var query = _context.Wagers
            .Include(w => w.Prediction)
            .Include(w => w.PredictionOption)
            .AsQueryable();
        if (chatId.HasValue)
            query = query.Where(w => w.chat_id == chatId.Value);
        if (gameId.HasValue)
            query = query.Where(w => w.Prediction.game_id == gameId.Value);

        var wagers = await query.OrderByDescending(w => w.placed_at).ToListAsync();
        var dtos = wagers.Select(w => new
        {
            bet_id = w.wager_id,
            w.chat_id,
            game_id = w.Prediction.game_id,
            user_id = w.user_id,
            bet_category = w.Prediction.prediction_type,
            prediction_details_json = JsonSerializer.Serialize(new
            {
                text = w.Prediction.title,
                dueBy = w.Prediction.lock_at.HasValue ? w.Prediction.lock_at.Value.ToString("O") : "",
                minPoints = w.PredictionOption.display_order,
                maxPoints = w.Prediction.pot_points,
            }),
            points_wagered = w.points_wagered,
            status = w.result_status,
            placed_at = w.placed_at,
            resolved_at = w.resolved_at,
        });
        return Ok(dtos);
    }

    // GET api/bets/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetBet(int id)
    {
        var wager = await _context.Wagers
            .Include(w => w.Prediction)
            .Include(w => w.PredictionOption)
            .FirstOrDefaultAsync(w => w.wager_id == id);
        if (wager is null)
            return NotFound();

        var dto = new
        {
            bet_id = wager.wager_id,
            wager.chat_id,
            game_id = wager.Prediction.game_id,
            user_id = wager.user_id,
            bet_category = wager.Prediction.prediction_type,
            prediction_details_json = JsonSerializer.Serialize(new
            {
                text = wager.Prediction.title,
                dueBy = wager.Prediction.lock_at.HasValue ? wager.Prediction.lock_at.Value.ToString("O") : "",
                minPoints = wager.PredictionOption.display_order,
                maxPoints = wager.Prediction.pot_points,
            }),
            points_wagered = wager.points_wagered,
            status = wager.result_status,
            placed_at = wager.placed_at,
            resolved_at = wager.resolved_at,
        };
        return Ok(dto);
    }
}

public class CreateBetRequest
{
    public int chat_id { get; set; }
    public int game_id { get; set; }
    public int user_id { get; set; }
    public string bet_category { get; set; } = null!;
    public string prediction_details_json { get; set; } = null!;
    public int points_wagered { get; set; }
    public string status { get; set; } = null!;
}
