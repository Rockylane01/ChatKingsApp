using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
    public int lifetime_points { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }

    // Stored in DB, never sent to client
    [JsonIgnore]
    public string? password_hash { get; set; }

    // Received from client on create/login, never stored directly
    [NotMapped]
    public string? password { get; set; }
}
