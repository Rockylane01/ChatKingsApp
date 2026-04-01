using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class ChatMember
{
    [Key]
    public int chat_member_id { get; set; }
    public int chat_id { get; set; }
    public int user_id { get; set; }
    public string role { get; set; } = null!;
    public int points_balance { get; set; }
    public DateTime joined_at { get; set; }
    public DateTime? left_at { get; set; }
    public bool is_active { get; set; }
    public bool is_king { get; set; }
    public string status { get; set; } = "active";
    public int? invited_by_user_id { get; set; }
}
