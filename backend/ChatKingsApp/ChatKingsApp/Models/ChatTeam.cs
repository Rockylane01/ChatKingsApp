using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class ChatTeam
{
    [Key]
    public int chat_team_id { get; set; }
    public int chat_id { get; set; }
    public int team_id { get; set; }
    public DateTime added_at { get; set; }
}

