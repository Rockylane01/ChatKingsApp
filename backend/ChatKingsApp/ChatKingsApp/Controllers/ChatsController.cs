using ChatKingsApp.Data;
using ChatKingsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatKingsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly ChatKingsDbContext _context;

    public ChatsController(ChatKingsDbContext context)
    {
        _context = context;
    }

    // GET api/chats?userId={id}
    // If userId is provided, returns chats the user belongs to.
    // Otherwise returns all chats.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Chat>>> GetChats([FromQuery] int? userId)
    {
        if (userId.HasValue)
        {
            var chatIds = await _context.ChatMembers
                .Where(cm => cm.user_id == userId.Value && cm.left_at == null)
                .Select(cm => cm.chat_id)
                .ToListAsync();

            var chats = await _context.Chats
                .Where(c => chatIds.Contains(c.chat_id))
                .OrderByDescending(c => c.updated_at)
                .ToListAsync();

            return Ok(chats);
        }

        return Ok(await _context.Chats.OrderByDescending(c => c.updated_at).ToListAsync());
    }

    // POST api/chats
    // Creates a new chat and adds the creator as the first member.
    [HttpPost]
    public async Task<ActionResult<Chat>> CreateChat([FromBody] Chat chat)
    {
        if (string.IsNullOrWhiteSpace(chat.chat_name))
            return BadRequest("chat_name is required.");

        if (chat.admin_id <= 0)
            return BadRequest("admin_id is required.");

        chat.chat_id = 0;
        chat.created_at = DateTime.UtcNow;
        chat.updated_at = DateTime.UtcNow;

        if (string.IsNullOrWhiteSpace(chat.bet_permission))
            chat.bet_permission = "all";

        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        // Auto-add the creator as first member
        var member = new ChatMember
        {
            chat_id = chat.chat_id,
            user_id = chat.admin_id,
            points = 0,
            joined_at = DateTime.UtcNow,
        };

        _context.ChatMembers.Add(member);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetChats), new { id = chat.chat_id }, chat);
    }

    // POST api/chats/{chatId}/join?userId={userId}
    [HttpPost("{chatId}/join")]
    public async Task<ActionResult> JoinChat(int chatId, [FromQuery] int userId)
    {
        var chat = await _context.Chats.FindAsync(chatId);
        if (chat is null)
            return NotFound("Chat not found.");

        var existing = await _context.ChatMembers
            .FirstOrDefaultAsync(cm => cm.chat_id == chatId && cm.user_id == userId && cm.left_at == null);

        if (existing is not null)
            return BadRequest("User is already a member of this chat.");

        var member = new ChatMember
        {
            chat_id = chatId,
            user_id = userId,
            points = 0,
            joined_at = DateTime.UtcNow,
        };

        _context.ChatMembers.Add(member);
        await _context.SaveChangesAsync();

        return Ok(member);
    }

    // GET api/chats/{chatId}/members
    [HttpGet("{chatId}/members")]
    public async Task<ActionResult> GetMembers(int chatId)
    {
        var members = await _context.ChatMembers
            .Where(cm => cm.chat_id == chatId && cm.left_at == null)
            .Join(
                _context.Users,
                cm => cm.user_id,
                u => u.user_id,
                (cm, u) => new
                {
                    cm.user_id,
                    u.username,
                    cm.points,
                    cm.joined_at,
                })
            .ToListAsync();

        return Ok(members);
    }
}
