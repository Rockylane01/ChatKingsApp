using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class ChatMember
{
    [Key]
    public int member_id { get; set; }
    public int chat_id { get; set; }
    public int user_id { get; set; }
    public int points { get; set; }
    public DateTime joined_at { get; set; }
    public DateTime? left_at { get; set; }
}

