using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class FriendRequest
{
    [Key]
    public int request_id { get; set; }
    public int sender_id { get; set; }
    public int receiver_id { get; set; }
    public string status { get; set; } = null!;
    public DateTime created_at { get; set; }
    public DateTime? responded_at { get; set; }
}

