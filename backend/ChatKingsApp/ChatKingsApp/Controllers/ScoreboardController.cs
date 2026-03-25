using ChatKingsApp.Models;
using ChatKingsApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatKingsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoreboardController : ControllerBase
{
    private readonly NcaamScoreboardService _ncaamScoreboard;

    public ScoreboardController(NcaamScoreboardService ncaamScoreboard)
    {
        _ncaamScoreboard = ncaamScoreboard;
    }

    /// <summary>NCAA men's basketball games for the home ticker: one ESPN scoreboard call, games in the next 7 Eastern calendar days.</summary>
    [HttpGet("ncaam")]
    [ProducesResponseType(typeof(IReadOnlyList<TickerGameDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TickerGameDto>>> GetNcaamTicker(CancellationToken cancellationToken)
    {
        var items = await _ncaamScoreboard.GetTickerGamesAsync(cancellationToken).ConfigureAwait(false);
        return Ok(items);
    }
}
