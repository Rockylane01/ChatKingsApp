using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatKingsApp.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    AuthProvider = table.Column<string>(type: "TEXT", nullable: false),
                    AuthProviderUserId = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "FRIENDSHIPS",
                columns: table => new
                {
                    RequestorUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddresseeUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FRIENDSHIPS", x => new { x.RequestorUserId, x.AddresseeUserId });
                    table.ForeignKey(
                        name: "FK_FRIENDSHIPS_USERS_AddresseeUserId",
                        column: x => x.AddresseeUserId,
                        principalTable: "USERS",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FRIENDSHIPS_USERS_RequestorUserId",
                        column: x => x.RequestorUserId,
                        principalTable: "USERS",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GAMES",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ScheduledFor = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GAMES", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_GAMES_USERS_HostUserId",
                        column: x => x.HostUserId,
                        principalTable: "USERS",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PUBLIC_PROFILES",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    AvatarUrl = table.Column<string>(type: "TEXT", nullable: true),
                    AboutMe = table.Column<string>(type: "TEXT", nullable: true),
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
                name: "CHAT_THREADS",
                columns: table => new
                {
                    ChatThreadId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatType = table.Column<string>(type: "TEXT", nullable: false),
                    GameId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Visibility = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                name: "TEAMS",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SlotNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    IsBot = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEAMS", x => x.TeamId);
                    table.ForeignKey(
                        name: "FK_TEAMS_GAMES_GameId",
                        column: x => x.GameId,
                        principalTable: "GAMES",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TURNS",
                columns: table => new
                {
                    GameTurnId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ActingUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: true)
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
                name: "MESSAGES",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatThreadId = table.Column<int>(type: "INTEGER", nullable: false),
                    SenderUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MESSAGES", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_MESSAGES_CHAT_THREADS_ChatThreadId",
                        column: x => x.ChatThreadId,
                        principalTable: "CHAT_THREADS",
                        principalColumn: "ChatThreadId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MESSAGES_USERS_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "USERS",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CHAT_TEAMS",
                columns: table => new
                {
                    ChatThreadId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHAT_TEAMS", x => new { x.ChatThreadId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_CHAT_TEAMS_CHAT_THREADS_ChatThreadId",
                        column: x => x.ChatThreadId,
                        principalTable: "CHAT_THREADS",
                        principalColumn: "ChatThreadId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CHAT_TEAMS_TEAMS_TeamId",
                        column: x => x.TeamId,
                        principalTable: "TEAMS",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GAME_STATS",
                columns: table => new
                {
                    GameStatId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GAME_STATS", x => x.GameStatId);
                    table.ForeignKey(
                        name: "FK_GAME_STATS_GAMES_GameId",
                        column: x => x.GameId,
                        principalTable: "GAMES",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GAME_STATS_TEAMS_TeamId",
                        column: x => x.TeamId,
                        principalTable: "TEAMS",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BOT_HISTORY",
                columns: table => new
                {
                    BotHistoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameTurnId = table.Column<int>(type: "INTEGER", nullable: false),
                    Prompt = table.Column<string>(type: "TEXT", nullable: false),
                    Response = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                name: "IX_CHAT_TEAMS_TeamId",
                table: "CHAT_TEAMS",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_CHAT_THREADS_GameId",
                table: "CHAT_THREADS",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_FRIENDSHIPS_AddresseeUserId",
                table: "FRIENDSHIPS",
                column: "AddresseeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GAME_STATS_GameId",
                table: "GAME_STATS",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GAME_STATS_TeamId",
                table: "GAME_STATS",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_GAMES_HostUserId",
                table: "GAMES",
                column: "HostUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGES_ChatThreadId",
                table: "MESSAGES",
                column: "ChatThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGES_SenderUserId",
                table: "MESSAGES",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OPEN_GameId",
                table: "OPEN",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_OPEN_HostUserId",
                table: "OPEN",
                column: "HostUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TEAMS_GameId",
                table: "TEAMS",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_TURNS_ActingUserId",
                table: "TURNS",
                column: "ActingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TURNS_GameId",
                table: "TURNS",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BOT_HISTORY");

            migrationBuilder.DropTable(
                name: "CHAT_TEAMS");

            migrationBuilder.DropTable(
                name: "FRIENDSHIPS");

            migrationBuilder.DropTable(
                name: "GAME_STATS");

            migrationBuilder.DropTable(
                name: "MESSAGES");

            migrationBuilder.DropTable(
                name: "OPEN");

            migrationBuilder.DropTable(
                name: "PUBLIC_PROFILES");

            migrationBuilder.DropTable(
                name: "TURNS");

            migrationBuilder.DropTable(
                name: "TEAMS");

            migrationBuilder.DropTable(
                name: "CHAT_THREADS");

            migrationBuilder.DropTable(
                name: "GAMES");

            migrationBuilder.DropTable(
                name: "USERS");
        }
    }
}
