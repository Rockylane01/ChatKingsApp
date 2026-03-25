using System.Text.Json.Serialization;

namespace ChatKingsApp.Models;

/// <summary>Wire shape matches frontend <c>TickerGame</c> (camelCase JSON keys).</summary>
public record TickerGameDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("league")] string League,
    [property: JsonPropertyName("matchup")] string Matchup,
    [property: JsonPropertyName("score")] string Score,
    [property: JsonPropertyName("status")] string Status);
