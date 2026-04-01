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
                .Where(cm => cm.user_id == userId.Value && cm.left_at == null && cm.status == "active")
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

        if (chat.created_by_user_id <= 0)
            return BadRequest("created_by_user_id is required.");

        chat.chat_id = 0;
        chat.chat_king_user_id = chat.created_by_user_id;
        chat.created_at = DateTime.UtcNow;
        chat.updated_at = DateTime.UtcNow;

        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        // Auto-add the creator as first member
        var member = new ChatMember
        {
            chat_id = chat.chat_id,
            user_id = chat.created_by_user_id,
            role = "admin",
            points_balance = 1000,
            joined_at = DateTime.UtcNow,
            is_active = true,
            is_king = true, // first member is King by default
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
            role = "member",
            points_balance = 1000,
            joined_at = DateTime.UtcNow,
            is_active = true,
        };

        _context.ChatMembers.Add(member);
        await _context.SaveChangesAsync();

        return Ok(member);
    }

    // GET api/chats/{chatId}/members
    [HttpGet("{chatId}/members")]
    public async Task<ActionResult> GetMembers(int chatId)
    {
        var chat = await _context.Chats.FindAsync(chatId);
        var kingUserId = chat?.chat_king_user_id;

        var members = await _context.ChatMembers
            .Where(cm => cm.chat_id == chatId && cm.left_at == null && cm.status == "active")
            .Join(
                _context.Users,
                cm => cm.user_id,
                u => u.user_id,
                (cm, u) => new
                {
                    cm.user_id,
                    u.username,
                    cm.points_balance,
                    is_king = kingUserId.HasValue ? cm.user_id == kingUserId : cm.is_king,
                    cm.joined_at,
                })
            .ToListAsync();

        return Ok(members);
    }

    // GET api/chats/{chatId}/leaderboard
    [HttpGet("{chatId}/leaderboard")]
    public async Task<ActionResult> GetLeaderboard(int chatId)
    {
        var leaderboard = await _context.ChatMembers
            .Where(cm => cm.chat_id == chatId && cm.is_active && cm.left_at == null)
            .Join(
                _context.Users,
                cm => cm.user_id,
                u => u.user_id,
                (cm, u) => new
                {
                    cm.user_id,
                    u.username,
                    cm.points_balance,
                    cm.is_king,
                })
            .OrderByDescending(m => m.points_balance)
            .ToListAsync();

        return Ok(leaderboard);
    }

    // GET api/chats/{chatId}/strikes?userId={userId}
    [HttpGet("{chatId}/strikes")]
    public async Task<ActionResult> GetStrikes(int chatId, [FromQuery] int userId)
    {
        var today = DateTime.UtcNow.Date;
        var count = await _context.StrikeEvents
            .Where(s => s.chat_id == chatId && s.user_id == userId && s.created_at >= today)
            .SumAsync(s => s.strike_value);

        return Ok(new { user_id = userId, chat_id = chatId, strikes_today = count, max_strikes = 3, locked = count >= 3 });
    }

    // GET api/chats/{chatId}/king
    [HttpGet("{chatId}/king")]
    public async Task<ActionResult> GetKing(int chatId)
    {
        var chat = await _context.Chats.FindAsync(chatId);
        if (chat is null)
            return NotFound("Chat not found.");

        if (chat.chat_king_user_id is null)
            return Ok(new { user_id = (int?)null, username = (string?)null });

        var king = await _context.Users.FindAsync(chat.chat_king_user_id.Value);
        if (king is null)
            return Ok(new { user_id = (int?)null, username = (string?)null });

        return Ok(new { king.user_id, king.username });
    }

    // POST api/chats/{chatId}/king/recalculate
    [HttpPost("{chatId}/king/recalculate")]
    public async Task<ActionResult> RecalculateKing(int chatId)
    {
        var chat = await _context.Chats.FindAsync(chatId);
        if (chat is null)
            return NotFound("Chat not found.");

        var topMember = await _context.ChatMembers
            .Where(cm => cm.chat_id == chatId && cm.left_at == null && cm.is_active)
            .OrderByDescending(cm => cm.points_balance)
            .FirstOrDefaultAsync();

        if (topMember is null)
            return Ok(new { user_id = (int?)null, username = (string?)null });

        // Tie-breaking: if current king is tied for highest, they keep the crown
        if (chat.chat_king_user_id.HasValue && chat.chat_king_user_id != topMember.user_id)
        {
            var currentKingMember = await _context.ChatMembers
                .FirstOrDefaultAsync(cm => cm.chat_id == chatId
                    && cm.user_id == chat.chat_king_user_id.Value
                    && cm.left_at == null && cm.is_active);

            if (currentKingMember != null && currentKingMember.points_balance >= topMember.points_balance)
            {
                // Current king is tied or ahead — keep the crown
                var existingKing = await _context.Users.FindAsync(chat.chat_king_user_id.Value);
                return Ok(new { existingKing?.user_id, existingKing?.username });
            }
        }

        chat.chat_king_user_id = topMember.user_id;
        chat.updated_at = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var newKing = await _context.Users.FindAsync(topMember.user_id);
        return Ok(new { newKing?.user_id, newKing?.username });
    }

    // POST api/chats/{chatId}/invite
    [HttpPost("{chatId}/invite")]
    public async Task<ActionResult> InviteUser(int chatId, [FromBody] InviteRequest req)
    {
        var chat = await _context.Chats.FindAsync(chatId);
        if (chat is null)
            return NotFound("Chat not found.");

        var existing = await _context.ChatMembers
            .FirstOrDefaultAsync(cm => cm.chat_id == chatId && cm.user_id == req.invited_user_id && cm.left_at == null);

        if (existing is not null)
            return BadRequest("User is already a member or has a pending invite.");

        var member = new ChatMember
        {
            chat_id = chatId,
            user_id = req.invited_user_id,
            role = "member",
            points_balance = 0,
            joined_at = DateTime.UtcNow,
            is_active = false,
            status = "invited",
            invited_by_user_id = req.invited_by_user_id,
        };

        _context.ChatMembers.Add(member);
        await _context.SaveChangesAsync();

        return Ok(member);
    }

    // GET api/chats/invitations?userId={}
    [HttpGet("invitations")]
    public async Task<ActionResult> GetInvitations([FromQuery] int userId)
    {
        var rows = await _context.ChatMembers
            .Where(cm => cm.user_id == userId && cm.status == "invited" && cm.left_at == null)
            .Join(_context.Chats, cm => cm.chat_id, c => c.chat_id,
                (cm, c) => new { c.chat_id, c.chat_name, c.created_at, cm.invited_by_user_id })
            .ToListAsync();

        var result = new List<object>();
        foreach (var row in rows)
        {
            string? invitedByUsername = null;
            if (row.invited_by_user_id.HasValue)
            {
                var inviter = await _context.Users.FindAsync(row.invited_by_user_id.Value);
                invitedByUsername = inviter?.username;
            }
            result.Add(new { row.chat_id, row.chat_name, row.created_at, invited_by_username = invitedByUsername });
        }

        return Ok(result);
    }

    // GET api/chats/{chatId}/pending-invites
    [HttpGet("{chatId}/pending-invites")]
    public async Task<ActionResult> GetPendingInvites(int chatId)
    {
        var rows = await _context.ChatMembers
            .Where(cm => cm.chat_id == chatId && cm.status == "invited" && cm.left_at == null)
            .Join(_context.Users, cm => cm.user_id, u => u.user_id,
                (cm, u) => new { u.user_id, u.username })
            .ToListAsync();

        return Ok(rows);
    }

    // POST api/chats/{chatId}/accept-invite?userId={}
    [HttpPost("{chatId}/accept-invite")]
    public async Task<ActionResult> AcceptInvite(int chatId, [FromQuery] int userId)
    {
        var member = await _context.ChatMembers
            .FirstOrDefaultAsync(cm => cm.chat_id == chatId && cm.user_id == userId && cm.status == "invited");

        if (member is null)
            return NotFound("Invitation not found.");

        member.status = "active";
        member.is_active = true;
        member.points_balance = 1000;
        member.joined_at = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(member);
    }

    // POST api/chats/{chatId}/decline-invite?userId={}
    [HttpPost("{chatId}/decline-invite")]
    public async Task<ActionResult> DeclineInvite(int chatId, [FromQuery] int userId)
    {
        var member = await _context.ChatMembers
            .FirstOrDefaultAsync(cm => cm.chat_id == chatId && cm.user_id == userId && cm.status == "invited");

        if (member is null)
            return NotFound("Invitation not found.");

        _context.ChatMembers.Remove(member);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST api/chats/{chatId}/leave?userId={}
    [HttpPost("{chatId}/leave")]
    public async Task<ActionResult> LeaveChat(int chatId, [FromQuery] int userId)
    {
        var member = await _context.ChatMembers
            .FirstOrDefaultAsync(cm => cm.chat_id == chatId && cm.user_id == userId && cm.left_at == null && cm.status == "active");

        if (member is null)
            return NotFound("You are not a member of this chat.");

        member.left_at = DateTime.UtcNow;
        member.is_active = false;
        member.status = "left";
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public record InviteRequest(int invited_user_id, int invited_by_user_id);
