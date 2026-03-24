using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatKingsApp.Migrations
{
    /// <inheritdoc />
    public partial class BasketballErdAlign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "passing_yards",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "rushing_yards",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "total_yards",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "time_of_possession_seconds",
                table: "game_team_stats");

            migrationBuilder.AddColumn<string>(
                name: "division",
                table: "teams",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "season",
                table: "games",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "assists",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "blocks",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "field_goals_attempted",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "field_goals_made",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fouls",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "free_throws_attempted",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "free_throws_made",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rebounds",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "steals",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "three_pointers_attempted",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "three_pointers_made",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "division",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "season",
                table: "games");

            migrationBuilder.DropColumn(
                name: "assists",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "blocks",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "field_goals_attempted",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "field_goals_made",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "fouls",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "free_throws_attempted",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "free_throws_made",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "rebounds",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "steals",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "three_pointers_attempted",
                table: "game_team_stats");

            migrationBuilder.DropColumn(
                name: "three_pointers_made",
                table: "game_team_stats");

            migrationBuilder.AddColumn<int>(
                name: "passing_yards",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rushing_yards",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "total_yards",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "time_of_possession_seconds",
                table: "game_team_stats",
                type: "INTEGER",
                nullable: true);
        }
    }
}
