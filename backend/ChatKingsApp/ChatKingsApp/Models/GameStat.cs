using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class GameStat
{
    [Key]
    public int stat_id { get; set; }
    public int game_id { get; set; }
    public int team_id { get; set; }
    public int? passing_yards { get; set; }
    public int? rushing_yards { get; set; }
    public int? total_yards { get; set; }
    public int? turnovers { get; set; }
    public int? time_of_possession_seconds { get; set; }
    public string? other_stats_json { get; set; }
}

