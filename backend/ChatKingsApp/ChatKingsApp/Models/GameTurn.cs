namespace ChatKingsApp.Models;

public class GameTurn
{
    public int GameTurnId { get; set; }
    public int GameId { get; set; }
    public int TurnNumber { get; set; }
    public int? ActingUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Summary { get; set; }

    public Game Game { get; set; } = null!;
    public User? ActingUser { get; set; }
}

