using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatKingsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddChatKingColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "chat_king_user_id",
                table: "chats",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "chat_king_user_id",
                table: "chats");
        }
    }
}
