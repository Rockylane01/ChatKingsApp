namespace ChatKingsApp.Models;

public class OpenGame
{
    public int OpenGameId { get; set; }
    public int GameId { get; set; }
    public int? HostUserId { get; set; }
    public string Visibility { get; set; } = "public";
    public DateTime CreatedAt { get; set; }

    public Game Game { get; set; } = null!;
    public User? HostUser { get; set; }
}

