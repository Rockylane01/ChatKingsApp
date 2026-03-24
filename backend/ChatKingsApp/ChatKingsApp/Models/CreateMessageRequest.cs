namespace ChatKingsApp.Models;

/// <summary>
/// Request DTO for creating a message. Frontend sends user_id; we map to sender_user_id.
/// </summary>
public class CreateMessageRequest
{
    public int chat_id { get; set; }
    public int user_id { get; set; }
    public string message_type { get; set; } = null!;
    public string message_text { get; set; } = null!;
    public int? prediction_id { get; set; }
}
