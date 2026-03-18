using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatKingsApp.Migrations
{
    /// <inheritdoc />
    public partial class Initial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CHAT_TEAMS_CHAT_THREADS_ChatThreadId",
                table: "CHAT_TEAMS");

            migrationBuilder.DropForeignKey(
                name: "FK_CHAT_TEAMS_TEAMS_TeamId",
                table: "CHAT_TEAMS");

            migrationBuilder.DropForeignKey(
                name: "FK_FRIENDSHIPS_USERS_AddresseeUserId",
                table: "FRIENDSHIPS");

            migrationBuilder.DropForeignKey(
                name: "FK_FRIENDSHIPS_USERS_RequestorUserId",
                table: "FRIENDSHIPS");

            migrationBuilder.DropForeignKey(
                name: "FK_GAME_STATS_GAMES_GameId",
                table: "GAME_STATS");

            migrationBuilder.DropForeignKey(
                name: "FK_GAME_STATS_TEAMS_TeamId",
                table: "GAME_STATS");

            migrationBuilder.DropForeignKey(
                name: "FK_GAMES_USERS_HostUserId",
                table: "GAMES");

            migrationBuilder.DropForeignKey(
                name: "FK_MESSAGES_CHAT_THREADS_ChatThreadId",
                table: "MESSAGES");

            migrationBuilder.DropForeignKey(
                name: "FK_MESSAGES_USERS_SenderUserId",
                table: "MESSAGES");

            migrationBuilder.DropForeignKey(
                name: "FK_TEAMS_GAMES_GameId",
                table: "TEAMS");

            migrationBuilder.DropTable(
                name: "BOT_HISTORY");

            migrationBuilder.DropTable(
                name: "CHAT_THREADS");

            migrationBuilder.DropTable(
                name: "OPEN");

            migrationBuilder.DropTable(
                name: "PUBLIC_PROFILES");

            migrationBuilder.DropTable(
                name: "TURNS");

            migrationBuilder.DropIndex(
                name: "IX_TEAMS_GameId",
                table: "TEAMS");

            migrationBuilder.DropIndex(
                name: "IX_MESSAGES_ChatThreadId",
                table: "MESSAGES");

            migrationBuilder.DropIndex(
                name: "IX_MESSAGES_SenderUserId",
                table: "MESSAGES");

            migrationBuilder.DropIndex(
                name: "IX_GAMES_HostUserId",
                table: "GAMES");

            migrationBuilder.DropIndex(
                name: "IX_GAME_STATS_GameId",
                table: "GAME_STATS");

            migrationBuilder.DropIndex(
                name: "IX_GAME_STATS_TeamId",
                table: "GAME_STATS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FRIENDSHIPS",
                table: "FRIENDSHIPS");

            migrationBuilder.DropIndex(
                name: "IX_FRIENDSHIPS_AddresseeUserId",
                table: "FRIENDSHIPS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CHAT_TEAMS",
                table: "CHAT_TEAMS");

            migrationBuilder.DropIndex(
                name: "IX_CHAT_TEAMS_TeamId",
                table: "CHAT_TEAMS");

            migrationBuilder.DropColumn(
                name: "AuthProvider",
                table: "USERS");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "TEAMS");

            migrationBuilder.DropColumn(
                name: "IsBot",
                table: "TEAMS");

            migrationBuilder.DropColumn(
                name: "SlotNumber",
                table: "TEAMS");

            migrationBuilder.DropColumn(
                name: "ChatThreadId",
                table: "MESSAGES");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "MESSAGES");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "GAMES");

            migrationBuilder.DropColumn(
                name: "ScheduledFor",
                table: "GAMES");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "GAME_STATS");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "GAME_STATS");

            migrationBuilder.DropColumn(
                name: "RequestedAt",
                table: "FRIENDSHIPS");

            migrationBuilder.DropColumn(
                name: "RespondedAt",
                table: "FRIENDSHIPS");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "USERS",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "USERS",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "USERS",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "LastLoginAt",
                table: "USERS",
                newName: "profile_image_url");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "USERS",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "USERS",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AuthProviderUserId",
                table: "USERS",
                newName: "add_code");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "USERS",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TEAMS",
                newName: "team_name");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "TEAMS",
                newName: "team_id");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "MESSAGES",
                newName: "sent_at");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "MESSAGES",
                newName: "message_type");

            migrationBuilder.RenameColumn(
                name: "SenderUserId",
                table: "MESSAGES",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "IsSystem",
                table: "MESSAGES",
                newName: "chat_id");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "MESSAGES",
                newName: "message_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "GAMES",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "GAMES",
                newName: "venue");

            migrationBuilder.RenameColumn(
                name: "HostUserId",
                table: "GAMES",
                newName: "home_team_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "GAMES",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "GAMES",
                newName: "game_id");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "GAME_STATS",
                newName: "team_id");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "GAME_STATS",
                newName: "game_id");

            migrationBuilder.RenameColumn(
                name: "GameStatId",
                table: "GAME_STATS",
                newName: "stat_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "FRIENDSHIPS",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AddresseeUserId",
                table: "FRIENDSHIPS",
                newName: "user_id_2");

            migrationBuilder.RenameColumn(
                name: "RequestorUserId",
                table: "FRIENDSHIPS",
                newName: "user_id_1");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "CHAT_TEAMS",
                newName: "team_id");

            migrationBuilder.RenameColumn(
                name: "ChatThreadId",
                table: "CHAT_TEAMS",
                newName: "chat_id");

            migrationBuilder.AddColumn<int>(
                name: "all_time_points",
                table: "USERS",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "conference",
                table: "TEAMS",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "league",
                table: "TEAMS",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "logo_url",
                table: "TEAMS",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "team_abbreviation",
                table: "TEAMS",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "message_text",
                table: "MESSAGES",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "related_bet_id",
                table: "MESSAGES",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "away_score",
                table: "GAMES",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "away_team_id",
                table: "GAMES",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "GAMES",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "game_datetime",
                table: "GAMES",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "home_score",
                table: "GAMES",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "other_stats_json",
                table: "GAME_STATS",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "passing_yards",
                table: "GAME_STATS",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rushing_yards",
                table: "GAME_STATS",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "time_of_possession_seconds",
                table: "GAME_STATS",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "total_yards",
                table: "GAME_STATS",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "turnovers",
                table: "GAME_STATS",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "friendship_id",
                table: "FRIENDSHIPS",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "chat_team_id",
                table: "CHAT_TEAMS",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "added_at",
                table: "CHAT_TEAMS",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_FRIENDSHIPS",
                table: "FRIENDSHIPS",
                column: "friendship_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CHAT_TEAMS",
                table: "CHAT_TEAMS",
                column: "chat_team_id");

            migrationBuilder.CreateTable(
                name: "BET_HISTORY",
                columns: table => new
                {
                    history_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false),
                    game_id = table.Column<int>(type: "INTEGER", nullable: false),
                    bet_id = table.Column<int>(type: "INTEGER", nullable: false),
                    points_change = table.Column<int>(type: "INTEGER", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BET_HISTORY", x => x.history_id);
                });

            migrationBuilder.CreateTable(
                name: "BETS",
                columns: table => new
                {
                    bet_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false),
                    game_id = table.Column<int>(type: "INTEGER", nullable: false),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    bet_category = table.Column<string>(type: "TEXT", nullable: false),
                    prediction_details_json = table.Column<string>(type: "TEXT", nullable: false),
                    points_wagered = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    placed_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    resolved_at = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BETS", x => x.bet_id);
                });

            migrationBuilder.CreateTable(
                name: "CHAT_MEMBERS",
                columns: table => new
                {
                    member_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    points = table.Column<int>(type: "INTEGER", nullable: false),
                    joined_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    left_at = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHAT_MEMBERS", x => x.member_id);
                });

            migrationBuilder.CreateTable(
                name: "CHATS",
                columns: table => new
                {
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    chat_name = table.Column<string>(type: "TEXT", nullable: true),
                    admin_id = table.Column<int>(type: "INTEGER", nullable: false),
                    end_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    bet_permission = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHATS", x => x.chat_id);
                });

            migrationBuilder.CreateTable(
                name: "DAILY_STRIKES",
                columns: table => new
                {
                    strike_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    chat_id = table.Column<int>(type: "INTEGER", nullable: false),
                    strike_date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    strike_count = table.Column<int>(type: "INTEGER", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DAILY_STRIKES", x => x.strike_id);
                });

            migrationBuilder.CreateTable(
                name: "FRIEND_REQUESTS",
                columns: table => new
                {
                    request_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    sender_id = table.Column<int>(type: "INTEGER", nullable: false),
                    receiver_id = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    responded_at = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FRIEND_REQUESTS", x => x.request_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BET_HISTORY");

            migrationBuilder.DropTable(
                name: "BETS");

            migrationBuilder.DropTable(
                name: "CHAT_MEMBERS");

            migrationBuilder.DropTable(
                name: "CHATS");

            migrationBuilder.DropTable(
                name: "DAILY_STRIKES");

            migrationBuilder.DropTable(
                name: "FRIEND_REQUESTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FRIENDSHIPS",
                table: "FRIENDSHIPS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CHAT_TEAMS",
                table: "CHAT_TEAMS");

            migrationBuilder.DropColumn(
                name: "all_time_points",
                table: "USERS");

            migrationBuilder.DropColumn(
                name: "conference",
                table: "TEAMS");

            migrationBuilder.DropColumn(
                name: "league",
                table: "TEAMS");

            migrationBuilder.DropColumn(
                name: "logo_url",
                table: "TEAMS");

            migrationBuilder.DropColumn(
                name: "team_abbreviation",
                table: "TEAMS");

            migrationBuilder.DropColumn(
                name: "message_text",
                table: "MESSAGES");

            migrationBuilder.DropColumn(
                name: "related_bet_id",
                table: "MESSAGES");

            migrationBuilder.DropColumn(
                name: "away_score",
                table: "GAMES");

            migrationBuilder.DropColumn(
                name: "away_team_id",
                table: "GAMES");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "GAMES");

            migrationBuilder.DropColumn(
                name: "game_datetime",
                table: "GAMES");

            migrationBuilder.DropColumn(
                name: "home_score",
                table: "GAMES");

            migrationBuilder.DropColumn(
                name: "other_stats_json",
                table: "GAME_STATS");

            migrationBuilder.DropColumn(
                name: "passing_yards",
                table: "GAME_STATS");

            migrationBuilder.DropColumn(
                name: "rushing_yards",
                table: "GAME_STATS");

            migrationBuilder.DropColumn(
                name: "time_of_possession_seconds",
                table: "GAME_STATS");

            migrationBuilder.DropColumn(
                name: "total_yards",
                table: "GAME_STATS");

            migrationBuilder.DropColumn(
                name: "turnovers",
                table: "GAME_STATS");

            migrationBuilder.DropColumn(
                name: "friendship_id",
                table: "FRIENDSHIPS");

            migrationBuilder.DropColumn(
                name: "chat_team_id",
                table: "CHAT_TEAMS");

            migrationBuilder.DropColumn(
                name: "added_at",
                table: "CHAT_TEAMS");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "USERS",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "USERS",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "USERS",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "profile_image_url",
                table: "USERS",
                newName: "LastLoginAt");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "USERS",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "USERS",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "add_code",
                table: "USERS",
                newName: "AuthProviderUserId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "USERS",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "team_name",
                table: "TEAMS",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "team_id",
                table: "TEAMS",
                newName: "TeamId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "MESSAGES",
                newName: "SenderUserId");

            migrationBuilder.RenameColumn(
                name: "sent_at",
                table: "MESSAGES",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "message_type",
                table: "MESSAGES",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "chat_id",
                table: "MESSAGES",
                newName: "IsSystem");

            migrationBuilder.RenameColumn(
                name: "message_id",
                table: "MESSAGES",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "GAMES",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "venue",
                table: "GAMES",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "GAMES",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "home_team_id",
                table: "GAMES",
                newName: "HostUserId");

            migrationBuilder.RenameColumn(
                name: "game_id",
                table: "GAMES",
                newName: "GameId");

            migrationBuilder.RenameColumn(
                name: "team_id",
                table: "GAME_STATS",
                newName: "TeamId");

            migrationBuilder.RenameColumn(
                name: "game_id",
                table: "GAME_STATS",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "stat_id",
                table: "GAME_STATS",
                newName: "GameStatId");

            migrationBuilder.RenameColumn(
                name: "user_id_2",
                table: "FRIENDSHIPS",
                newName: "AddresseeUserId");

            migrationBuilder.RenameColumn(
                name: "user_id_1",
                table: "FRIENDSHIPS",
                newName: "RequestorUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "FRIENDSHIPS",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "team_id",
                table: "CHAT_TEAMS",
                newName: "TeamId");

            migrationBuilder.RenameColumn(
                name: "chat_id",
                table: "CHAT_TEAMS",
                newName: "ChatThreadId");

            migrationBuilder.AddColumn<string>(
                name: "AuthProvider",
                table: "USERS",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "TEAMS",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBot",
                table: "TEAMS",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SlotNumber",
                table: "TEAMS",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChatThreadId",
                table: "MESSAGES",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "MESSAGES",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "GAMES",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledFor",
                table: "GAMES",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "GAME_STATS",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "GAME_STATS",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedAt",
                table: "FRIENDSHIPS",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RespondedAt",
                table: "FRIENDSHIPS",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FRIENDSHIPS",
                table: "FRIENDSHIPS",
                columns: new[] { "RequestorUserId", "AddresseeUserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CHAT_TEAMS",
                table: "CHAT_TEAMS",
                columns: new[] { "ChatThreadId", "TeamId" });

            migrationBuilder.CreateTable(
                name: "CHAT_THREADS",
                columns: table => new
                {
                    ChatThreadId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: true),
                    ChatType = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHAT_THREADS", x => x.ChatThreadId);
                    table.ForeignKey(
                        name: "FK_CHAT_THREADS_GAMES_GameId",
                        column: x => x.GameId,
                        principalTable: "GAMES",
                        principalColumn: "GameId");
                });

            migrationBuilder.CreateTable(
                name: "OPEN",
                columns: table => new
                {
                    OpenGameId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    HostUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Visibility = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OPEN", x => x.OpenGameId);
                    table.ForeignKey(
                        name: "FK_OPEN_GAMES_GameId",
                        column: x => x.GameId,
                        principalTable: "GAMES",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OPEN_USERS_HostUserId",
                        column: x => x.HostUserId,
                        principalTable: "USERS",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "PUBLIC_PROFILES",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AboutMe = table.Column<string>(type: "TEXT", nullable: true),
                    AvatarUrl = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    Visibility = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PUBLIC_PROFILES", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_PUBLIC_PROFILES_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "USERS",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TURNS",
                columns: table => new
                {
                    GameTurnId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActingUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: true),
                    TurnNumber = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TURNS", x => x.GameTurnId);
                    table.ForeignKey(
                        name: "FK_TURNS_GAMES_GameId",
                        column: x => x.GameId,
                        principalTable: "GAMES",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TURNS_USERS_ActingUserId",
                        column: x => x.ActingUserId,
                        principalTable: "USERS",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "BOT_HISTORY",
                columns: table => new
                {
                    BotHistoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameTurnId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Prompt = table.Column<string>(type: "TEXT", nullable: false),
                    Response = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOT_HISTORY", x => x.BotHistoryId);
                    table.ForeignKey(
                        name: "FK_BOT_HISTORY_GAMES_GameId",
                        column: x => x.GameId,
                        principalTable: "GAMES",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BOT_HISTORY_TURNS_GameTurnId",
                        column: x => x.GameTurnId,
                        principalTable: "TURNS",
                        principalColumn: "GameTurnId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BOT_HISTORY_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "USERS",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TEAMS_GameId",
                table: "TEAMS",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGES_ChatThreadId",
                table: "MESSAGES",
                column: "ChatThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGES_SenderUserId",
                table: "MESSAGES",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GAMES_HostUserId",
                table: "GAMES",
                column: "HostUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GAME_STATS_GameId",
                table: "GAME_STATS",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GAME_STATS_TeamId",
                table: "GAME_STATS",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_FRIENDSHIPS_AddresseeUserId",
                table: "FRIENDSHIPS",
                column: "AddresseeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CHAT_TEAMS_TeamId",
                table: "CHAT_TEAMS",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_BOT_HISTORY_GameId",
                table: "BOT_HISTORY",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_BOT_HISTORY_GameTurnId",
                table: "BOT_HISTORY",
                column: "GameTurnId");

            migrationBuilder.CreateIndex(
                name: "IX_BOT_HISTORY_UserId",
                table: "BOT_HISTORY",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CHAT_THREADS_GameId",
                table: "CHAT_THREADS",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_OPEN_GameId",
                table: "OPEN",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_OPEN_HostUserId",
                table: "OPEN",
                column: "HostUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TURNS_ActingUserId",
                table: "TURNS",
                column: "ActingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TURNS_GameId",
                table: "TURNS",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_CHAT_TEAMS_CHAT_THREADS_ChatThreadId",
                table: "CHAT_TEAMS",
                column: "ChatThreadId",
                principalTable: "CHAT_THREADS",
                principalColumn: "ChatThreadId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CHAT_TEAMS_TEAMS_TeamId",
                table: "CHAT_TEAMS",
                column: "TeamId",
                principalTable: "TEAMS",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FRIENDSHIPS_USERS_AddresseeUserId",
                table: "FRIENDSHIPS",
                column: "AddresseeUserId",
                principalTable: "USERS",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FRIENDSHIPS_USERS_RequestorUserId",
                table: "FRIENDSHIPS",
                column: "RequestorUserId",
                principalTable: "USERS",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GAME_STATS_GAMES_GameId",
                table: "GAME_STATS",
                column: "GameId",
                principalTable: "GAMES",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GAME_STATS_TEAMS_TeamId",
                table: "GAME_STATS",
                column: "TeamId",
                principalTable: "TEAMS",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GAMES_USERS_HostUserId",
                table: "GAMES",
                column: "HostUserId",
                principalTable: "USERS",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MESSAGES_CHAT_THREADS_ChatThreadId",
                table: "MESSAGES",
                column: "ChatThreadId",
                principalTable: "CHAT_THREADS",
                principalColumn: "ChatThreadId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MESSAGES_USERS_SenderUserId",
                table: "MESSAGES",
                column: "SenderUserId",
                principalTable: "USERS",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TEAMS_GAMES_GameId",
                table: "TEAMS",
                column: "GameId",
                principalTable: "GAMES",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
