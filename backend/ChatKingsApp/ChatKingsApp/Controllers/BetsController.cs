using ChatKingsApp.Data;
using ChatKingsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatKingsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BetsController : ControllerBase
{
    private static readonly HashSet<string> ValidStatuses = ["pending", "won", "lost", "cancelled"];

    private readonly ChatKingsDbContext _context;

    public BetsController(ChatKingsDbContext context)
    {
        _context = context;
    }

    // POST api/bets
    [HttpPost]
    public async Task<ActionResult<Bet>> CreateBet([FromBody] Bet bet)
    {
        if (!ValidStatuses.Contains(bet.status))
            return BadRequest($"Invalid status '{bet.status}'. Must be one of: {string.Join(", ", ValidStatuses)}.");

        bet.bet_id = 0;
        bet.placed_at = DateTime.UtcNow;

        _context.Bets.Add(bet);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBet), new { id = bet.bet_id }, bet);
    }

    // PUT api/bets/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<Bet>> UpdateBet(int id, [FromBody] Bet bet)
    {
        if (!ValidStatuses.Contains(bet.status))
            return BadRequest($"Invalid status '{bet.status}'. Must be one of: {string.Join(", ", ValidStatuses)}.");

        var existing = await _context.Bets.FindAsync(id);
        if (existing is null)
            return NotFound();

        existing.chat_id = bet.chat_id;
        existing.game_id = bet.game_id;
        existing.user_id = bet.user_id;
        existing.bet_category = bet.bet_category;
        existing.prediction_details_json = bet.prediction_details_json;
        existing.points_wagered = bet.points_wagered;
        existing.status = bet.status;
        existing.resolved_at = bet.resolved_at;

        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    // DELETE api/bets/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBet(int id)
    {
        var bet = await _context.Bets.FindAsync(id);
        if (bet is null)
            return NotFound();

        _context.Bets.Remove(bet);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET api/bets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Bet>>> GetBets()
    {
        return Ok(await _context.Bets.ToListAsync());
    }

    // GET api/bets/{id} — used by CreatedAtAction in POST
    [HttpGet("{id}")]
    public async Task<ActionResult<Bet>> GetBet(int id)
    {
        var bet = await _context.Bets.FindAsync(id);
        return bet is null ? NotFound() : Ok(bet);
    }
}
