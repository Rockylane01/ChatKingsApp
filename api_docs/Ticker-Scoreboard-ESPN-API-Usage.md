# Ticker scoreboard — ESPN API usage (ChatKings)

This document describes how the ChatKings app uses ESPN’s **unofficial** Site API for the authenticated home page scrolling score ticker. It focuses on **what we call**, **how often**, and **how to test without excessive requests**.

For general endpoint reference, see [Basketball API Documentation.md](./Basketball%20API%20Documentation.md), [College Sports Documentation.md](./College%20Sports%20Documentation.md), and [Response Schemas.md](./Response%20Schemas.md).

---

## Scope

- **Sport:** Basketball (`basketball`).
- **League:** NCAA men’s only — slug `mens-college-basketball`.
- **Data needed for the ticker:** league label, matchup, score line, status (see `events` → `competitions` → `competitors` and `status` in [Response Schemas.md](./Response%20Schemas.md)).

We do **not** call team rosters, summaries, standings, or other resources for the ticker.

---

## Endpoint we use (single request)

```http
GET https://site.api.espn.com/apis/site/v2/sports/basketball/mens-college-basketball/scoreboard
```

**No query string.** We intentionally avoid:

- **`dates=YYYYMMDD-YYYYMMDD`** — hyphen ranges return **404** for NCAAM (unlike NBA), so they cannot supply a week in one call.
- **Multiple `dates=YYYYMMDD` requests** — would multiply traffic; the app uses **one** GET per home load.

ESPN’s default scoreboard returns a **current slate** (often one primary calendar day plus nearby tip times). The backend **filters** those `events` to tipoffs that fall on a **US Eastern calendar date** in **[today, today + 7 days)** (seven days including today). Games **outside** that week are dropped; games **inside** the week but **not present** in ESPN’s default payload **do not appear** (API limitation).

---

## ChatKings implementation (request count)

| Layer | Behavior | ESPN requests |
|--------|-----------|----------------|
| **Backend** | `NcaamScoreboardService` issues **one** `GET` to the scoreboard URL above, parses `events`, keeps games in the **7-day Eastern** window, sorts by start time. | **1** per call to our API |
| **Frontend** | `HomePage` calls **`GET /api/scoreboard/ncaam` once** on mount (`useEffect` with an empty dependency array). Navigating away and back triggers **another** mount and **one** more ESPN call via the backend. | **0** to ESPN (browser only talks to our API) |

**Summary:** Each home dashboard load costs **one** upstream ESPN scoreboard GET. There is **no** polling interval in the current UI.

---

## Minimal testing (avoid hammering ESPN)

1. **Exploring payloads:** a single curl is enough, e.g.  
   `curl -sS "https://site.api.espn.com/apis/site/v2/sports/basketball/mens-college-basketball/scoreboard"`

2. **Testing our backend:** `GET http://localhost:5166/api/scoreboard/ncaam` triggers **one** upstream request. Avoid tight loops / auto-refresh.

3. **No documented quota:** ESPN does not publish official limits for these endpoints. Treat them as **shared, undocumented infrastructure** — keep volume low during development.

4. **Optional hardening (not implemented):** A **short TTL cache** on `GET /api/scoreboard/ncaam` (e.g. 60–120 seconds) or **mock JSON** in dev reduces repeat hits while testing.

---

## Our API surface (for the frontend)

The React app does not call ESPN; it calls:

```http
GET /api/scoreboard/ncaam
```

Response: JSON array of `{ id, league, matchup, score, status }`, derived from filtered ESPN `events` (see `TickerGameDto` and `NcaamScoreboardService` in the backend).

---

## Field mapping (reference)

| Ticker field | ESPN source (conceptual) |
|--------------|---------------------------|
| `league` | `leagues[0].abbreviation` |
| `matchup` | `events[].shortName` (normalized) or `events[].name` |
| `score` | Away/home `competitors[].score`; scheduled 0–0 shown as `—` |
| `status` | `events[].status.type.shortDetail` / `detail`, with period + clock when live |

---

## Files in this repo

- Backend: `Services/NcaamScoreboardService.cs`, `Controllers/ScoreboardController.cs`, `Models/TickerGameDto.cs`
- Frontend: `HomePage.tsx` (fetch on mount)
- Dev proxy: `frontend/vite.config.ts` proxies `/api` to the backend
