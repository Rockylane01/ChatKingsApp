namespace ChatKingsApp.Models;

public class GameStat
{
    public int GameStatId { get; set; }
    public int GameId { get; set; }
    public int TeamId { get; set; }
    public int Score { get; set; }
    public int Rank { get; set; }

    public Game Game { get; set; } = null!;
    public Team Team { get; set; } = null!;
}

