using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class Message
{
    [Key]
    public int message_id { get; set; }
    public int chat_id { get; set; }
    public int user_id { get; set; }
    public string message_type { get; set; } = null!;
    public string message_text { get; set; } = null!;
    public int? related_bet_id { get; set; }
    public DateTime sent_at { get; set; }
}

