using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class ChatLeaderboardSnapshot
{
    [Key]
    public int snapshot_id { get; set; }
    public int chat_id { get; set; }
    public int user_id { get; set; }
    public int rank_position { get; set; }
    public int points_balance { get; set; }
    public DateOnly snapshot_date { get; set; }
}
