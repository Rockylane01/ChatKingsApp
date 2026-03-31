using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class Chat
{
    [Key]
    public int chat_id { get; set; }
    public string? chat_name { get; set; }
    public string chat_type { get; set; } = null!;
    public int created_by_user_id { get; set; }
    public string status { get; set; } = null!;
    public string timezone { get; set; } = "America/New_York";
    public int? chat_king_user_id { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
