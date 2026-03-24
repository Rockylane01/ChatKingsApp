using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class GameTeamStat
{
    [Key]
    public int stat_id { get; set; }
    public int game_id { get; set; }
    public int team_id { get; set; }
    public int? field_goals_made { get; set; }
    public int? field_goals_attempted { get; set; }
    public int? three_pointers_made { get; set; }
    public int? three_pointers_attempted { get; set; }
    public int? free_throws_made { get; set; }
    public int? free_throws_attempted { get; set; }
    public int? rebounds { get; set; }
    public int? assists { get; set; }
    public int? steals { get; set; }
    public int? blocks { get; set; }
    public int? turnovers { get; set; }
    public int? fouls { get; set; }
}
