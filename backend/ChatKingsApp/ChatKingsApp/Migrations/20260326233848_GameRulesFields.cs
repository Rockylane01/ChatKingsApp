using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatKingsApp.Migrations
{
    /// <inheritdoc />
    public partial class GameRulesFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "espn_event_id",
                table: "predictions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "initial_bet_max",
                table: "predictions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "initial_bet_min",
                table: "predictions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "timezone",
                table: "chats",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "is_king",
                table: "chat_members",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "espn_event_id",
                table: "predictions");

            migrationBuilder.DropColumn(
                name: "initial_bet_max",
                table: "predictions");

            migrationBuilder.DropColumn(
                name: "initial_bet_min",
                table: "predictions");

            migrationBuilder.DropColumn(
                name: "timezone",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "is_king",
                table: "chat_members");
        }
    }
}
