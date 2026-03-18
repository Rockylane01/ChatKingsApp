namespace ChatKingsApp.Models;

public class ChatThread
{
    public int ChatThreadId { get; set; }
    public string ChatType { get; set; } = "game";
    public int? GameId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Game? Game { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<ChatTeam> ChatTeams { get; set; } = new List<ChatTeam>();
}

