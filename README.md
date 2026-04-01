# ChatKings

ChatKings is a social sports prediction app designed as a fun, points-based alternative to real-money sports betting. Users compete with friends in group chats by making predictions on live sports outcomes, earning points for correct picks rather than wagering real money.

## Features

- **User Authentication** — Sign in with an existing account to access the app
- **Group Chats** — Create and join group chats with other users
- **Real-Time Messaging** — Send and receive messages within group chats
- **Sports Predictions** — Make predictions on upcoming sports matchups (e.g., NBA games) and compete for points
- **Points System** — Track points balances per chat with a leaderboard-style experience

## Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) (v18+)
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Start the Backend

```bash
cd backend/ChatKingsApp/ChatKingsApp
dotnet run
```

The API will start at `http://localhost:5135`.

### Backend Database Configuration

The backend supports SQLite for development and PostgreSQL for production.

- Local development defaults to SQLite (`ChatKings.db`) via `appsettings.Development.json`.
- Production defaults to PostgreSQL and requires `ConnectionStrings__DefaultConnection`.
- You can override provider selection with `DatabaseProvider` (`Sqlite` or `Postgres`).

Example production env:

```bash
ASPNETCORE_ENVIRONMENT=Production
DatabaseProvider=Postgres
ConnectionStrings__DefaultConnection=Host=<host>;Port=5432;Database=<db>;Username=<user>;Password=<password>;SSL Mode=Require;Trust Server Certificate=true
```

### Start the Frontend

```bash
cd frontend
npm install
npm run dev
```

The app will be available at `http://localhost:5173`.

### Try It Out

1. Sign in with the test account: **test@example.com**
2. Create a new group chat or join an existing one
3. Send messages in the chat
4. Open a prediction, select a sports matchup, and submit your pick

## Current Status

The core features listed above are fully implemented and working end to end. Additional functionality is planned, including:

- King of the Chat role and point-based rankings
- Prediction resolution and automated point awards
- Friend requests and direct messaging
- API integration with live sports data providers

## Requirements (EARS)

### Ubiquitous
1. The system shall allow users to participate in group chats with persistent point totals.
2. The system shall assign the King of the Chat role to the user with the highest point total.
3. The system shall allow chats to persist for a configurable duration up to one year.

### Event Driven
4. When a correct answer is selected by a minority of participants, the system shall award bonus points.
5. When the King of the Chat creates a prediction question, the system shall require the King to place a wager.
6. When a prediction question is resolved, the system shall award points to users with correct answers.
7. When the King of the Chat is inactive beyond a configured time limit, the system shall revoke the King role and adjust point totals accordingly.
8. When a bet is ended and the King has not bet on it, the King shall automatically lose 10 points.

### State Driven
9. While a user holds the King of the Chat role, the system shall identify them as King.
10. While a prediction question is active, the system shall display the total points wagered without revealing distribution by answer.

