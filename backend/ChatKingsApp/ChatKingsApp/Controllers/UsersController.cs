using System.Security.Cryptography;
using ChatKingsApp.Data;
using ChatKingsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatKingsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ChatKingsDbContext _context;

    public UsersController(ChatKingsDbContext context)
    {
        _context = context;
    }

    // POST api/users
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] User user)
    {
        if (string.IsNullOrWhiteSpace(user.username))
            return BadRequest("username is required.");

        if (string.IsNullOrWhiteSpace(user.email))
            return BadRequest("email is required.");

        if (string.IsNullOrWhiteSpace(user.add_code))
            return BadRequest("add_code is required.");

        if (string.IsNullOrWhiteSpace(user.password))
            return BadRequest("password is required.");

        user.user_id = 0;
        user.password_hash = HashPassword(user.password);
        user.created_at = DateTime.UtcNow;
        user.updated_at = DateTime.UtcNow;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.user_id }, user);
    }

    // POST api/users/login
    [HttpPost("login")]
    public async Task<ActionResult<User>> Login([FromBody] LoginRequest req)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.username == req.Username);

        if (user is null || !VerifyPassword(req.Password, user.password_hash ?? ""))
            return Unauthorized("Invalid username or password.");

        return Ok(user);
    }

    // PUT api/users/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User user)
    {
        var existing = await _context.Users.FindAsync(id);
        if (existing is null)
            return NotFound();

        existing.username = user.username;
        existing.email = user.email;
        existing.phone_number = user.phone_number;
        existing.add_code = user.add_code;
        existing.profile_image_url = user.profile_image_url;
        existing.lifetime_points = user.lifetime_points;
        existing.updated_at = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(user.password))
            existing.password_hash = HashPassword(user.password);

        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    // DELETE api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return Ok(await _context.Users.ToListAsync());
    }

    // GET api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    // GET api/users/by-add-code/{addCode}
    [HttpGet("by-add-code/{addCode}")]
    public async Task<ActionResult<User>> GetUserByAddCode(string addCode)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.add_code == addCode);
        return user is null ? NotFound() : Ok(user);
    }

    private static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string stored)
    {
        var parts = stored.Split(':');
        if (parts.Length != 2) return false;
        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] expectedHash = Convert.FromBase64String(parts[1]);
        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }
}

public record LoginRequest(string Username, string Password);
