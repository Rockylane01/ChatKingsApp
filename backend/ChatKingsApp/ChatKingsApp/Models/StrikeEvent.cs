using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class StrikeEvent
{
    [Key]
    public int strike_event_id { get; set; }
    public int user_id { get; set; }
    public int chat_id { get; set; }
    public string reason { get; set; } = null!;
    public int strike_value { get; set; }
    public DateTime created_at { get; set; }
}
