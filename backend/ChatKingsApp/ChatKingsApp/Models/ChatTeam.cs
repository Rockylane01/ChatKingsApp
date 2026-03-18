namespace ChatKingsApp.Models;

public class ChatTeam
{
    public int ChatThreadId { get; set; }
    public int TeamId { get; set; }

    public ChatThread ChatThread { get; set; } = null!;
    public Team Team { get; set; } = null!;
}

