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
    public async Task<ActionResult<object>> SendMessage([FromBody] CreateMessageRequest req)
    {
        if (!ValidMessageTypes.Contains(req.message_type))
            return BadRequest($"Invalid message_type '{req.message_type}'. Must be one of: {string.Join(", ", ValidMessageTypes)}.");

        if (string.IsNullOrWhiteSpace(req.message_text))
            return BadRequest("message_text is required.");

        var message = new ChatMessage
        {
            chat_id = req.chat_id,
            sender_user_id = req.user_id,
            message_type = req.message_type,
            message_text = req.message_text,
            prediction_id = req.prediction_id,
            created_at = DateTime.UtcNow,
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();

        var dto = new
        {
            message.message_id,
            message.chat_id,
            user_id = message.sender_user_id,
            message.message_type,
            message.message_text,
            message.prediction_id,
            sent_at = message.created_at,
        };
        return CreatedAtAction(nameof(GetMessage), new { id = message.message_id }, dto);
    }

    // GET api/messages?chatId={id}&after={messageId}
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetMessages(
        [FromQuery] int? chatId, [FromQuery] int? after)
    {
        var query = _context.ChatMessages.AsQueryable();

        if (chatId.HasValue)
            query = query.Where(m => m.chat_id == chatId.Value);

        if (after.HasValue)
            query = query.Where(m => m.message_id > after.Value);

        var messages = await query.OrderBy(m => m.created_at).ToListAsync();
        // Map to frontend shape: user_id, sent_at
        var dtos = messages.Select(m => new
        {
            m.message_id,
            m.chat_id,
            user_id = m.sender_user_id,
            m.message_type,
            m.message_text,
            m.prediction_id,
            sent_at = m.created_at,
        });
        return Ok(dtos);
    }

    // GET api/messages/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetMessage(int id)
    {
        var message = await _context.ChatMessages.FindAsync(id);
        if (message is null)
            return NotFound();
        return Ok(new
        {
            message.message_id,
            message.chat_id,
            user_id = message.sender_user_id,
            message.message_type,
            message.message_text,
            message.prediction_id,
            sent_at = message.created_at,
        });
    }

    // DELETE api/messages/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var message = await _context.ChatMessages.FindAsync(id);
        if (message is null)
            return NotFound();

        _context.ChatMessages.Remove(message);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
