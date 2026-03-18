using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class Bet
{
    [Key]
    public int bet_id { get; set; }

    [Range(1, int.MaxValue)]
    public int chat_id { get; set; }

    [Range(1, int.MaxValue)]
    public int game_id { get; set; }

    [Range(1, int.MaxValue)]
    public int user_id { get; set; }

    [Required]
    public string bet_category { get; set; } = null!;

    [Required]
    public string prediction_details_json { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int points_wagered { get; set; }

    [Required]
    public string status { get; set; } = null!;

    public DateTime placed_at { get; set; }
    public DateTime? resolved_at { get; set; }
}

