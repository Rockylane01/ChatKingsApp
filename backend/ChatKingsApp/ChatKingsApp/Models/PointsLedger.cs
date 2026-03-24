using System.ComponentModel.DataAnnotations;

namespace ChatKingsApp.Models;

public class PointsLedger
{
    [Key]
    public int ledger_id { get; set; }
    public int user_id { get; set; }
    public int chat_id { get; set; }
    public int? prediction_id { get; set; }
    public int? wager_id { get; set; }
    public int change_amount { get; set; }
    public string change_reason { get; set; } = null!;
    public DateTime created_at { get; set; }
}
