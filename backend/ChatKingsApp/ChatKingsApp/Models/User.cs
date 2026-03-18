using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class User
{
    [Key]
    public int user_id { get; set; }
    public string username { get; set; } = null!;
    public string email { get; set; } = null!;
    public string? phone_number { get; set; }
    public string add_code { get; set; } = null!;
    public string? profile_image_url { get; set; }
    public int all_time_points { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}

