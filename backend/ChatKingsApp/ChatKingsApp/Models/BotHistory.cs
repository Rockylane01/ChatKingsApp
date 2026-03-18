namespace ChatKingsApp.Models;

public class BotHistory
{
    public int BotHistoryId { get; set; }
    public int UserId { get; set; }
    public int GameId { get; set; }
    public int GameTurnId { get; set; }
    public string Prompt { get; set; } = null!;
    public string Response { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Game Game { get; set; } = null!;
    public GameTurn GameTurn { get; set; } = null!;
}

