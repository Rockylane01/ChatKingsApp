using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class Prediction
{
    [Key]
    public int prediction_id { get; set; }
    public int chat_id { get; set; }
    public int created_by_user_id { get; set; }
    public int game_id { get; set; }
    public string title { get; set; } = null!;
    public string? description { get; set; }
    public string prediction_type { get; set; } = null!;
    public string status { get; set; } = null!;
    public int pot_points { get; set; }
    public DateTime created_at { get; set; }
    public DateTime? lock_at { get; set; }
    public DateTime? resolved_at { get; set; }
    public string? espn_event_id { get; set; }
    public int initial_bet_min { get; set; } = 20;
    public int initial_bet_max { get; set; } = 100;

    public ICollection<PredictionOption> Options { get; set; } = new List<PredictionOption>();
    public ICollection<Wager> Wagers { get; set; } = new List<Wager>();
}
