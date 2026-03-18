using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class Bet
{
    [Key]
    public int bet_id { get; set; }
    public int chat_id { get; set; }
    public int game_id { get; set; }
    public int user_id { get; set; }
    public string bet_category { get; set; } = null!;
    public string prediction_details_json { get; set; } = null!;
    public int points_wagered { get; set; }
    public string status { get; set; } = null!;
    public DateTime placed_at { get; set; }
    public DateTime? resolved_at { get; set; }
}

