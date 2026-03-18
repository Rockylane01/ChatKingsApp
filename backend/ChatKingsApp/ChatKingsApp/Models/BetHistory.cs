using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class BetHistory
{
    [Key]
    public int history_id { get; set; }
    public int user_id { get; set; }
    public int chat_id { get; set; }
    public int game_id { get; set; }
    public int bet_id { get; set; }
    public int points_change { get; set; }
    public DateTime recorded_at { get; set; }
}

