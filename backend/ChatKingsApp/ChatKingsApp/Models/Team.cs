using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class Team
{
    [Key]
    public int team_id { get; set; }
    public string team_name { get; set; } = null!;
    public string team_abbreviation { get; set; } = null!;
    public string? logo_url { get; set; }
    public string league { get; set; } = null!;
    public string? conference { get; set; }
}

