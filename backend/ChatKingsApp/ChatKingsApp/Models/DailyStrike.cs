using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class DailyStrike
{
    [Key]
    public int strike_id { get; set; }
    public int user_id { get; set; }
    public int chat_id { get; set; }
    public DateOnly strike_date { get; set; }
    public int strike_count { get; set; }
    public DateTime updated_at { get; set; }
}

