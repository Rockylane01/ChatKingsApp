using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class Friendship
{
    [Key]
    public int friendship_id { get; set; }
    public int user_id_1 { get; set; }
    public int user_id_2 { get; set; }
    public DateTime created_at { get; set; }
}

