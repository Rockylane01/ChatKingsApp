namespace ChatKingsApp.Models;

public class Game
{
    public int GameId { get; set; }
    public int HostUserId { get; set; }
    public string Status { get; set; } = "open";
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ScheduledFor { get; set; }

    public User HostUser { get; set; } = null!;
    public ICollection<GameTurn> Turns { get; set; } = new List<GameTurn>();
    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<GameStat> GameStats { get; set; } = new List<GameStat>();
    public ICollection<ChatThread> ChatThreads { get; set; } = new List<ChatThread>();
    public ICollection<OpenGame> OpenGames { get; set; } = new List<OpenGame>();
}

