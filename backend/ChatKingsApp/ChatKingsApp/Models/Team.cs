using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class Team
{
    [Key]
    public int team_id { get; set; }
    public string team_name { get; set; } = null!;
    public string? team_abbreviation { get; set; }
    public string? league { get; set; }
    public string? conference { get; set; }
    public string? division { get; set; }
}
