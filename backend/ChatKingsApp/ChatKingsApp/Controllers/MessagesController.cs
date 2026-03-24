using ChatKingsApp.Data;
using ChatKingsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatKingsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private static readonly HashSet<string> ValidMessageTypes = ["text", "bet"];

    private readonly ChatKingsDbContext _context;

    public MessagesController(ChatKingsDbContext context)
    {
        _context = context;
    }

    // POST api/messages
    [HttpPost]
    public async Task<ActionResult<Message>> SendMessage([FromBody] Message message)
    {
        if (!ValidMessageTypes.Contains(message.message_type))
            return BadRequest($"Invalid message_type '{message.message_type}'. Must be one of: {string.Join(", ", ValidMessageTypes)}.");

        if (string.IsNullOrWhiteSpace(message.message_text))
            return BadRequest("message_text is required.");

        message.message_id = 0;
        message.sent_at = DateTime.UtcNow;

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMessage), new { id = message.message_id }, message);
    }

    // GET api/messages?chatId={id}&after={messageId}
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessages(
        [FromQuery] int? chatId, [FromQuery] int? after)
    {
        var query = _context.Messages.AsQueryable();

        if (chatId.HasValue)
            query = query.Where(m => m.chat_id == chatId.Value);

        if (after.HasValue)
            query = query.Where(m => m.message_id > after.Value);

        var messages = await query.OrderBy(m => m.sent_at).ToListAsync();
        return Ok(messages);
    }

    // GET api/messages/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetMessage(int id)
    {
        var message = await _context.Messages.FindAsync(id);
        return message is null ? NotFound() : Ok(message);
    }

    // DELETE api/messages/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var message = await _context.Messages.FindAsync(id);
        if (message is null)
            return NotFound();

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
