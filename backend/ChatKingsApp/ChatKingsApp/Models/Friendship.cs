namespace ChatKingsApp.Models;

public class Friendship
{
    public int RequestorUserId { get; set; }
    public int AddresseeUserId { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime RequestedAt { get; set; }
    public DateTime? RespondedAt { get; set; }

    public User RequestorUser { get; set; } = null!;
    public User AddresseeUser { get; set; } = null!;
}

