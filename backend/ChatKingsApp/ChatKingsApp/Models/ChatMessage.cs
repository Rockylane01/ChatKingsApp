using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class ChatMessage
{
    [Key]
    public int message_id { get; set; }
    public int chat_id { get; set; }
    public int sender_user_id { get; set; }
    public string message_type { get; set; } = null!;
    public string message_text { get; set; } = null!;
    public int? prediction_id { get; set; }
    public DateTime created_at { get; set; }
}
