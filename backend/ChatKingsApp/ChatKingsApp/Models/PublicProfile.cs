namespace ChatKingsApp.Models;

public class PublicProfile
{
    public int UserId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string? AboutMe { get; set; }
    public string Visibility { get; set; } = "public";

    public User User { get; set; } = null!;
}

