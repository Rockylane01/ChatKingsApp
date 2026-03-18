namespace ChatKingsApp.Models;

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string AuthProvider { get; set; } = null!;
    public string AuthProviderUserId { get; set; } = null!;
    public string? DisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string Status { get; set; } = "active";

    public PublicProfile? PublicProfile { get; set; }
    public ICollection<Message> MessagesSent { get; set; } = new List<Message>();
    public ICollection<Friendship> FriendshipsRequested { get; set; } = new List<Friendship>();
    public ICollection<Friendship> FriendshipsReceived { get; set; } = new List<Friendship>();
    public ICollection<Game> GamesHosted { get; set; } = new List<Game>();
    public ICollection<GameTurn> GameTurns { get; set; } = new List<GameTurn>();
    public ICollection<BotHistory> BotHistories { get; set; } = new List<BotHistory>();
}

