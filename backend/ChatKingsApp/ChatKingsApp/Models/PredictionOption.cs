using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class PredictionOption
{
    [Key]
    public int option_id { get; set; }
    public int prediction_id { get; set; }
    public string option_label { get; set; } = null!;
    public int? team_id { get; set; }
    public int display_order { get; set; }

    public ICollection<Wager> Wagers { get; set; } = new List<Wager>();
}
