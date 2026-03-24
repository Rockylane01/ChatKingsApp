using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class PredictionResolution
{
    [Key]
    public int resolution_id { get; set; }
    public int prediction_id { get; set; }
    public int winning_option_id { get; set; }
    public int resolved_by_user_id { get; set; }
    public string? notes { get; set; }
    public DateTime resolved_at { get; set; }
}
