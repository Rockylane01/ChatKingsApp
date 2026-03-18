using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class Game
{
    [Key]
    public int game_id { get; set; }
    public int home_team_id { get; set; }
    public int away_team_id { get; set; }
    public DateTime game_datetime { get; set; }
    public string? venue { get; set; }
    public int? home_score { get; set; }
    public int? away_score { get; set; }
    public string status { get; set; } = null!;
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}

