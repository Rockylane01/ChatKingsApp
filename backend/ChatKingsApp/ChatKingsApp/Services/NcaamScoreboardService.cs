using System.Globalization;
using System.Text.Json;
using ChatKingsApp.Models;

namespace ChatKingsApp.Services;

public class NcaamScoreboardService
{
    /// <summary>
    /// No <c>?dates=</c>: hyphen date ranges return 404 for NCAAM; a single day omits other days.
    /// Default scoreboard returns ESPN's current slate (often multiple days of games in one payload).
    /// </summary>
    private const string ScoreboardUrl =
        "https://site.api.espn.com/apis/site/v2/sports/basketball/mens-college-basketball/scoreboard";
    private static readonly TimeZoneInfo Eastern = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

    private readonly HttpClient _http;

    public NcaamScoreboardService(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// One upstream GET to ESPN. Keeps games whose start time falls on a calendar day in US Eastern
    /// in the window [today ET, today ET + 7 days) (seven full calendar days, including today).
    /// </summary>
    public async Task<IReadOnlyList<TickerGameDto>> GetTickerGamesAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync(ScoreboardUrl, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return [];

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        var root = doc.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
            return [];

        if (root.TryGetProperty("code", out _))
            return [];

        var leagueAbbr = "NCAAM";
        if (root.TryGetProperty("leagues", out var leagues) && leagues.ValueKind == JsonValueKind.Array && leagues.GetArrayLength() > 0)
        {
            var L = leagues[0];
            if (L.TryGetProperty("abbreviation", out var abEl))
                leagueAbbr = abEl.GetString() ?? leagueAbbr;
        }

        if (!root.TryGetProperty("events", out var events) || events.ValueKind != JsonValueKind.Array)
            return [];

        var nowEt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern);
        var windowStartEt = nowEt.Date;
        var windowEndEtExclusive = windowStartEt.AddDays(7);

        var list = new List<ParsedTicker>();
        foreach (var ev in events.EnumerateArray())
        {
            var startUtc = ParseEventStartUtc(ev);
            if (startUtc == null)
                continue;

            var startEt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(startUtc.Value, DateTimeKind.Utc), Eastern);
            if (startEt < windowStartEt || startEt >= windowEndEtExclusive)
                continue;

            var mapped = MapEvent(ev, leagueAbbr, startUtc.Value);
            if (mapped != null)
                list.Add(mapped);
        }

        return list
            .OrderBy(x => x.StartUtc)
            .Select(x => new TickerGameDto(x.Id, x.League, x.Matchup, x.Score, x.Status))
            .ToList();
    }

    private static ParsedTicker? MapEvent(JsonElement ev, string leagueAbbr, DateTime startUtc)
    {
        if (!ev.TryGetProperty("id", out var idEl))
            return null;
        var id = idEl.GetString();
        if (string.IsNullOrEmpty(id))
            return null;

        var matchup = BuildMatchup(ev);
        if (!ev.TryGetProperty("competitions", out var comps) || comps.ValueKind != JsonValueKind.Array || comps.GetArrayLength() == 0)
            return new ParsedTicker(id, leagueAbbr, matchup, "—", "—", startUtc);

        var competition = comps[0];
        if (!competition.TryGetProperty("competitors", out var competitors) || competitors.ValueKind != JsonValueKind.Array)
            return new ParsedTicker(id, leagueAbbr, matchup, "—", BuildStatus(ev), startUtc);

        JsonElement? away = null;
        JsonElement? home = null;
        foreach (var c in competitors.EnumerateArray())
        {
            if (!c.TryGetProperty("homeAway", out var haEl))
                continue;
            var ha = haEl.GetString();
            if (ha == "away")
                away = c;
            else if (ha == "home")
                home = c;
        }

        var scoreLine = BuildScoreLine(ev, away, home);
        var status = BuildStatus(ev);

        return new ParsedTicker(id, leagueAbbr, matchup, scoreLine, status, startUtc);
    }

    private static DateTime? ParseEventStartUtc(JsonElement ev)
    {
        if (!ev.TryGetProperty("date", out var d) || d.ValueKind != JsonValueKind.String)
            return null;
        return DateTime.TryParse(d.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dt)
            ? dt
            : null;
    }

    private static string BuildMatchup(JsonElement ev)
    {
        if (ev.TryGetProperty("shortName", out var sn) && sn.ValueKind == JsonValueKind.String)
        {
            var s = sn.GetString();
            if (!string.IsNullOrWhiteSpace(s))
                return s.Replace(" VS ", " vs ", StringComparison.OrdinalIgnoreCase);
        }

        if (ev.TryGetProperty("name", out var n) && n.ValueKind == JsonValueKind.String)
        {
            var s = n.GetString();
            if (!string.IsNullOrWhiteSpace(s))
                return s;
        }

        return "Game";
    }

    private static string BuildScoreLine(JsonElement ev, JsonElement? away, JsonElement? home)
    {
        if (away == null || home == null)
            return "—";

        var ascore = ReadScore(away.Value);
        var hscore = ReadScore(home.Value);

        var state = "";
        if (ev.TryGetProperty("status", out var st) && st.TryGetProperty("type", out var ty) && ty.TryGetProperty("state", out var se))
            state = se.GetString() ?? "";

        if (state == "pre" && ascore == "0" && hscore == "0")
            return "—";

        return $"{ascore} - {hscore}";
    }

    private static string ReadScore(JsonElement competitor)
    {
        if (!competitor.TryGetProperty("score", out var sc))
            return "0";
        return sc.ValueKind switch
        {
            JsonValueKind.String => sc.GetString() ?? "0",
            JsonValueKind.Number => sc.GetRawText(),
            _ => "0",
        };
    }

    private static string BuildStatus(JsonElement ev)
    {
        if (!ev.TryGetProperty("status", out var status))
            return "—";

        if (!status.TryGetProperty("type", out var type))
            return "—";

        var state = type.TryGetProperty("state", out var se) ? se.GetString() : "";

        if (type.TryGetProperty("shortDetail", out var sd) && sd.ValueKind == JsonValueKind.String)
        {
            var shortD = sd.GetString();
            if (!string.IsNullOrWhiteSpace(shortD))
            {
                if (state == "in" && status.TryGetProperty("displayClock", out var clock) && clock.ValueKind == JsonValueKind.String)
                {
                    var clk = clock.GetString();
                    if (!string.IsNullOrWhiteSpace(clk) && clk != "0.0" && clk != "0:00")
                    {
                        if (status.TryGetProperty("period", out var per) && per.ValueKind == JsonValueKind.Number)
                        {
                            var p = per.GetInt32();
                            if (p > 0)
                                return $"{FormatPeriod(p)} {clk}";
                        }
                    }
                }

                return shortD;
            }
        }

        if (type.TryGetProperty("detail", out var d) && d.ValueKind == JsonValueKind.String)
        {
            var detail = d.GetString();
            if (!string.IsNullOrWhiteSpace(detail))
                return detail;
        }

        return "—";
    }

    private static string FormatPeriod(int period) => period switch
    {
        1 => "1st",
        2 => "2nd",
        3 => "3rd",
        4 => "4th",
        _ => $"{period}th",
    };

    private sealed record ParsedTicker(string Id, string League, string Matchup, string Score, string Status, DateTime StartUtc);
}
