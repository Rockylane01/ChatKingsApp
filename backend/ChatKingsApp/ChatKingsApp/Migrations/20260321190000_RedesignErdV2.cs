using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatKingsApp.Migrations
{
    /// <inheritdoc />
    public partial class RedesignErdV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Rename old tables to _old for data migration
            migrationBuilder.RenameTable(name: "USERS", newName: "USERS_old");
            migrationBuilder.RenameTable(name: "FRIEND_REQUESTS", newName: "FRIEND_REQUESTS_old");
            migrationBuilder.RenameTable(name: "FRIENDSHIPS", newName: "FRIENDSHIPS_old");
            migrationBuilder.RenameTable(name: "CHATS", newName: "CHATS_old");
            migrationBuilder.RenameTable(name: "CHAT_MEMBERS", newName: "CHAT_MEMBERS_old");
            migrationBuilder.RenameTable(name: "MESSAGES", newName: "MESSAGES_old");
            migrationBuilder.RenameTable(name: "TEAMS", newName: "TEAMS_old");
            migrationBuilder.RenameTable(name: "GAMES", newName: "GAMES_old");
            migrationBuilder.RenameTable(name: "GAME_STATS", newName: "GAME_STATS_old");
            migrationBuilder.RenameTable(name: "CHAT_TEAMS", newName: "CHAT_TEAMS_old");
            migrationBuilder.RenameTable(name: "BETS", newName: "BETS_old");
            migrationBuilder.RenameTable(name: "DAILY_STRIKES", newName: "DAILY_STRIKES_old");
            migrationBuilder.RenameTable(name: "BET_HISTORY", newName: "BET_HISTORY_old");

            // 2. Create new tables
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    username = table.Column<string>(type: "TEXT", nullable: false),
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    phone_number = table.Column<string>(type: "TEXT", nullable: true),
                    add_code = table.Column<string>(type: "TEXT", nullable: false),
                    profile_image_url = table.Column<string>(type: "TEXT", nullable: true),
                    lifetime_points = table.Column<int>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_users", x => x.user_id));

            migrationBuilder.CreateTable(
                name: "friend_requests",
                columns: table => new
                {
                    request_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    sender_id = table.Column<int>(type: "INTEGER", nullable: false),
                    receiver_id = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    responded_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                },
                constraints: table => table.PrimaryKey("PK_friend_requests", x => x.request_id));

            migrationBuilder.CreateTable(
                name: "friendships",
                columns: table => new
                {
                    friendship_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id_1 = table.Column<int>(type: "INTEGER", nullable: false),
                    user_id_2 = table.Column<int>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_friendships", x => x.friendship_id));

            migrationBuilder.CreateTable(
                name: "chats",
                columns: table => new
                {
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    chat_name = table.Column<string>(type: "TEXT", nullable: true),
                    chat_type = table.Column<string>(type: "TEXT", nullable: false),
                    created_by_user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_chats", x => x.chat_id));

            migrationBuilder.CreateTable(
                name: "chat_members",
                columns: table => new
                {
                    chat_member_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    role = table.Column<string>(type: "TEXT", nullable: false),
                    points_balance = table.Column<int>(type: "INTEGER", nullable: false),
                    joined_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    left_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_chat_members", x => x.chat_member_id));

            migrationBuilder.CreateTable(
                name: "chat_messages",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false),
                    sender_user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    message_type = table.Column<string>(type: "TEXT", nullable: false),
                    message_text = table.Column<string>(type: "TEXT", nullable: false),
                    prediction_id = table.Column<int>(type: "INTEGER", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_chat_messages", x => x.message_id));

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    team_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    team_name = table.Column<string>(type: "TEXT", nullable: false),
                    team_abbreviation = table.Column<string>(type: "TEXT", nullable: true),
                    league = table.Column<string>(type: "TEXT", nullable: true),
                    conference = table.Column<string>(type: "TEXT", nullable: true),
                },
                constraints: table => table.PrimaryKey("PK_teams", x => x.team_id));

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    game_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    home_team_id = table.Column<int>(type: "INTEGER", nullable: false),
                    away_team_id = table.Column<int>(type: "INTEGER", nullable: false),
                    game_datetime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    venue = table.Column<string>(type: "TEXT", nullable: true),
                    home_score = table.Column<int>(type: "INTEGER", nullable: true),
                    away_score = table.Column<int>(type: "INTEGER", nullable: true),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_games", x => x.game_id));

            migrationBuilder.CreateTable(
                name: "game_team_stats",
                columns: table => new
                {
                    stat_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    game_id = table.Column<int>(type: "INTEGER", nullable: false),
                    team_id = table.Column<int>(type: "INTEGER", nullable: false),
                    passing_yards = table.Column<int>(type: "INTEGER", nullable: true),
                    rushing_yards = table.Column<int>(type: "INTEGER", nullable: true),
                    total_yards = table.Column<int>(type: "INTEGER", nullable: true),
                    turnovers = table.Column<int>(type: "INTEGER", nullable: true),
                    time_of_possession_seconds = table.Column<int>(type: "INTEGER", nullable: true),
                },
                constraints: table => table.PrimaryKey("PK_game_team_stats", x => x.stat_id));

            migrationBuilder.CreateTable(
                name: "chat_teams",
                columns: table => new
                {
                    chat_team_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false),
                    team_id = table.Column<int>(type: "INTEGER", nullable: false),
                    added_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_chat_teams", x => x.chat_team_id));

            migrationBuilder.CreateTable(
                name: "predictions",
                columns: table => new
                {
                    prediction_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false),
                    created_by_user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    game_id = table.Column<int>(type: "INTEGER", nullable: false),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    prediction_type = table.Column<string>(type: "TEXT", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    pot_points = table.Column<int>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    lock_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                },
                constraints: table => table.PrimaryKey("PK_predictions", x => x.prediction_id));

            migrationBuilder.CreateTable(
                name: "prediction_options",
                columns: table => new
                {
                    option_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    prediction_id = table.Column<int>(type: "INTEGER", nullable: false),
                    option_label = table.Column<string>(type: "TEXT", nullable: false),
                    team_id = table.Column<int>(type: "INTEGER", nullable: true),
                    display_order = table.Column<int>(type: "INTEGER", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_prediction_options", x => x.option_id));

            migrationBuilder.CreateTable(
                name: "wagers",
                columns: table => new
                {
                    wager_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    prediction_id = table.Column<int>(type: "INTEGER", nullable: false),
                    option_id = table.Column<int>(type: "INTEGER", nullable: false),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false),
                    points_wagered = table.Column<int>(type: "INTEGER", nullable: false),
                    result_status = table.Column<string>(type: "TEXT", nullable: false),
                    placed_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    resolved_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                },
                constraints: table => table.PrimaryKey("PK_wagers", x => x.wager_id));

            migrationBuilder.CreateTable(
                name: "prediction_resolutions",
                columns: table => new
                {
                    resolution_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    prediction_id = table.Column<int>(type: "INTEGER", nullable: false),
                    winning_option_id = table.Column<int>(type: "INTEGER", nullable: false),
                    resolved_by_user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    notes = table.Column<string>(type: "TEXT", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_prediction_resolutions", x => x.resolution_id));

            migrationBuilder.CreateTable(
                name: "strike_events",
                columns: table => new
                {
                    strike_event_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false),
                    reason = table.Column<string>(type: "TEXT", nullable: false),
                    strike_value = table.Column<int>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_strike_events", x => x.strike_event_id));

            migrationBuilder.CreateTable(
                name: "points_ledger",
                columns: table => new
                {
                    ledger_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false),
                    prediction_id = table.Column<int>(type: "INTEGER", nullable: true),
                    wager_id = table.Column<int>(type: "INTEGER", nullable: true),
                    change_amount = table.Column<int>(type: "INTEGER", nullable: false),
                    change_reason = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_points_ledger", x => x.ledger_id));

            migrationBuilder.CreateTable(
                name: "chat_leaderboard_snapshots",
                columns: table => new
                {
                    snapshot_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    rank_position = table.Column<int>(type: "INTEGER", nullable: false),
                    points_balance = table.Column<int>(type: "INTEGER", nullable: false),
                    snapshot_date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_chat_leaderboard_snapshots", x => x.snapshot_id));

            // 3. Migrate data
            migrationBuilder.Sql(@"
                INSERT INTO users (user_id, username, email, phone_number, add_code, profile_image_url, lifetime_points, created_at, updated_at)
                SELECT user_id, username, email, phone_number, add_code, profile_image_url, COALESCE(all_time_points, 0), created_at, updated_at FROM USERS_old
            ");
            migrationBuilder.Sql(@"
                INSERT INTO friend_requests (request_id, sender_id, receiver_id, status, created_at, responded_at)
                SELECT request_id, sender_id, receiver_id, status, created_at, responded_at FROM FRIEND_REQUESTS_old
            ");
            migrationBuilder.Sql(@"
                INSERT INTO friendships (friendship_id, user_id_1, user_id_2, created_at)
                SELECT friendship_id, user_id_1, user_id_2, created_at FROM FRIENDSHIPS_old
            ");
            migrationBuilder.Sql(@"
                INSERT INTO chats (chat_id, chat_name, chat_type, created_by_user_id, status, created_at, updated_at)
                SELECT chat_id, chat_name, COALESCE(bet_permission, 'group'), admin_id, COALESCE(bet_permission, 'active'), created_at, updated_at FROM CHATS_old
            ");
            migrationBuilder.Sql(@"
                INSERT INTO chat_members (chat_member_id, chat_id, user_id, role, points_balance, joined_at, left_at, is_active)
                SELECT member_id, chat_id, user_id, 'member', COALESCE(points, 0), joined_at, left_at, 1 FROM CHAT_MEMBERS_old
            ");
            migrationBuilder.Sql(@"
                INSERT INTO chat_messages (message_id, chat_id, sender_user_id, message_type, message_text, prediction_id, created_at)
                SELECT message_id, chat_id, user_id, COALESCE(message_type, 'text'), message_text, related_bet_id, sent_at FROM MESSAGES_old
            ");
            migrationBuilder.Sql(@"
                INSERT INTO teams (team_id, team_name, team_abbreviation, league, conference)
                SELECT team_id, team_name, COALESCE(team_abbreviation, ''), COALESCE(league, ''), conference FROM TEAMS_old
            ");
            migrationBuilder.Sql(@"
                INSERT INTO games (game_id, home_team_id, away_team_id, game_datetime, venue, home_score, away_score, status, created_at, updated_at)
                SELECT game_id, home_team_id, away_team_id, game_datetime, venue, home_score, away_score, status, created_at, updated_at FROM GAMES_old
            ");
            migrationBuilder.Sql(@"
                INSERT INTO game_team_stats (stat_id, game_id, team_id, passing_yards, rushing_yards, total_yards, turnovers, time_of_possession_seconds)
                SELECT stat_id, game_id, team_id, passing_yards, rushing_yards, total_yards, turnovers, time_of_possession_seconds FROM GAME_STATS_old
            ");
            migrationBuilder.Sql(@"
                INSERT INTO chat_teams (chat_team_id, chat_id, team_id, added_at)
                SELECT chat_team_id, chat_id, team_id, added_at FROM CHAT_TEAMS_old
            ");

            // BETS -> predictions + prediction_options + wagers
            migrationBuilder.Sql(@"
                INSERT INTO predictions (prediction_id, chat_id, created_by_user_id, game_id, title, description, prediction_type, status, pot_points, created_at, lock_at, resolved_at)
                SELECT bet_id, chat_id, user_id, game_id,
                    COALESCE(json_extract(prediction_details_json, '$.text'), bet_category, 'Prediction'),
                    json_extract(prediction_details_json, '$.text'),
                    bet_category, status,
                    COALESCE(json_extract(prediction_details_json, '$.maxPoints'), points_wagered, 100),
                    placed_at,
                    CASE WHEN json_extract(prediction_details_json, '$.dueBy') IS NOT NULL AND json_extract(prediction_details_json, '$.dueBy') != '' 
                         THEN datetime(json_extract(prediction_details_json, '$.dueBy')) ELSE NULL END,
                    resolved_at
                FROM BETS_old
            ");
            migrationBuilder.Sql(@"
                INSERT INTO prediction_options (option_id, prediction_id, option_label, team_id, display_order)
                SELECT bet_id, bet_id,
                    COALESCE(json_extract(prediction_details_json, '$.text'), bet_category, 'Option'),
                    NULL,
                    COALESCE(json_extract(prediction_details_json, '$.minPoints'), 10)
                FROM BETS_old
            ");
            migrationBuilder.Sql(@"
                INSERT INTO wagers (wager_id, prediction_id, option_id, user_id, chat_id, points_wagered, result_status, placed_at, resolved_at)
                SELECT b.bet_id, b.bet_id, b.bet_id, b.user_id, b.chat_id, b.points_wagered, b.status, b.placed_at, b.resolved_at
                FROM BETS_old b
            ");

            // Fix prediction_options.option_id and wagers.option_id - we used bet_id which collides. Need to fix.
            // Actually the option_id we inserted = bet_id, and prediction_id = bet_id. So each bet has pred_id=bet_id, option_id=bet_id. The wager has prediction_id=bet_id, option_id=bet_id. So we have a problem: option_id in prediction_options is the PK and we set it to bet_id. And we have one option per bet. So option_id = bet_id for the option, and wager.option_id = bet_id. That works! Good.

            migrationBuilder.Sql(@"
                INSERT INTO strike_events (strike_event_id, user_id, chat_id, reason, strike_value, created_at)
                SELECT strike_id, user_id, chat_id, 'daily_strike', strike_count, updated_at FROM DAILY_STRIKES_old
            ");
            migrationBuilder.Sql(@"
                INSERT INTO points_ledger (ledger_id, user_id, chat_id, prediction_id, wager_id, change_amount, change_reason, created_at)
                SELECT history_id, user_id, chat_id, bet_id, bet_id, points_change, 'bet_resolution', COALESCE(recorded_at, datetime('now')) FROM BET_HISTORY_old
            ");

            // 4. Drop old tables
            migrationBuilder.DropTable("USERS_old");
            migrationBuilder.DropTable("FRIEND_REQUESTS_old");
            migrationBuilder.DropTable("FRIENDSHIPS_old");
            migrationBuilder.DropTable("CHATS_old");
            migrationBuilder.DropTable("CHAT_MEMBERS_old");
            migrationBuilder.DropTable("MESSAGES_old");
            migrationBuilder.DropTable("TEAMS_old");
            migrationBuilder.DropTable("GAMES_old");
            migrationBuilder.DropTable("GAME_STATS_old");
            migrationBuilder.DropTable("CHAT_TEAMS_old");
            migrationBuilder.DropTable("BETS_old");
            migrationBuilder.DropTable("DAILY_STRIKES_old");
            migrationBuilder.DropTable("BET_HISTORY_old");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Down would revert - not fully implemented for brevity
            throw new NotImplementedException("Down migration not implemented for RedesignErdV2.");
        }
    }
}
