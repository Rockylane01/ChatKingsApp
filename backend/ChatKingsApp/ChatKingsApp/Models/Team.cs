namespace ChatKingsApp.Models;

public class Team
{
    public int TeamId { get; set; }
    public int GameId { get; set; }
    public string Name { get; set; } = null!;
    public int SlotNumber { get; set; }
    public bool IsBot { get; set; }

    public Game Game { get; set; } = null!;
    public ICollection<GameStat> GameStats { get; set; } = new List<GameStat>();
    public ICollection<ChatTeam> ChatTeams { get; set; } = new List<ChatTeam>();
}

