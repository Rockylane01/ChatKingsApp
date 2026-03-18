namespace ChatKingsApp.Models;

public class Message
{
    public int MessageId { get; set; }
    public int ChatThreadId { get; set; }
    public int SenderUserId { get; set; }
    public string Text { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsSystem { get; set; }

    public ChatThread ChatThread { get; set; } = null!;
    public User SenderUser { get; set; } = null!;
}

