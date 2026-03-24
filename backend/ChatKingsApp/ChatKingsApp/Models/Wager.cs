using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class Wager
{
    [Key]
    public int wager_id { get; set; }
    public int prediction_id { get; set; }
    public int option_id { get; set; }
    public int user_id { get; set; }
    public int chat_id { get; set; }
    public int points_wagered { get; set; }
    public string result_status { get; set; } = null!;
    public DateTime placed_at { get; set; }
    public DateTime? resolved_at { get; set; }

    public Prediction Prediction { get; set; } = null!;
    public PredictionOption PredictionOption { get; set; } = null!;
}
