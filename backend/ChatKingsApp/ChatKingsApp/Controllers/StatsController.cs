using ChatKingsApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatKingsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly ChatKingsDbContext _context;

    public StatsController(ChatKingsDbContext context)
    {
        _context = context;
    }

    /// <summary>All-time count of wagers placed across all users and chats.</summary>
    [HttpGet("wagers-total")]
    [ProducesResponseType(typeof(WagersTotalResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<WagersTotalResponse>> GetWagersTotal(CancellationToken cancellationToken)
    {
        var total = await _context.Wagers.CountAsync(cancellationToken).ConfigureAwait(false);
        return Ok(new WagersTotalResponse(total));
    }

    public sealed record WagersTotalResponse(int totalWagers);
}
