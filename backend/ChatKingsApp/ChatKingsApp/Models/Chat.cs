using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class Chat
{
    [Key]
    public int chat_id { get; set; }
    public string? chat_name { get; set; }
    public int admin_id { get; set; }
    public DateTime? end_date { get; set; }
    public string bet_permission { get; set; } = null!;
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}

