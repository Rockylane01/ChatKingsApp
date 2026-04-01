using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatKingsApp.MigrationsPostgres
{
    /// <inheritdoc />
    public partial class AddMissingChatMemberColumnsPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "invited_by_user_id",
                table: "chat_members",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "chat_members",
                type: "text",
                nullable: false,
                defaultValue: "active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "invited_by_user_id",
                table: "chat_members");

            migrationBuilder.DropColumn(
                name: "status",
                table: "chat_members");
        }
    }
}
