using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatKingsApp.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureWagerRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_wagers_option_id",
                table: "wagers",
                column: "option_id");

            migrationBuilder.CreateIndex(
                name: "IX_wagers_prediction_id",
                table: "wagers",
                column: "prediction_id");

            migrationBuilder.CreateIndex(
                name: "IX_prediction_options_prediction_id",
                table: "prediction_options",
                column: "prediction_id");

            migrationBuilder.AddForeignKey(
                name: "FK_prediction_options_predictions_prediction_id",
                table: "prediction_options",
                column: "prediction_id",
                principalTable: "predictions",
                principalColumn: "prediction_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_wagers_prediction_options_option_id",
                table: "wagers",
                column: "option_id",
                principalTable: "prediction_options",
                principalColumn: "option_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_wagers_predictions_prediction_id",
                table: "wagers",
                column: "prediction_id",
                principalTable: "predictions",
                principalColumn: "prediction_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prediction_options_predictions_prediction_id",
                table: "prediction_options");

            migrationBuilder.DropForeignKey(
                name: "FK_wagers_prediction_options_option_id",
                table: "wagers");

            migrationBuilder.DropForeignKey(
                name: "FK_wagers_predictions_prediction_id",
                table: "wagers");

            migrationBuilder.DropIndex(
                name: "IX_wagers_option_id",
                table: "wagers");

            migrationBuilder.DropIndex(
                name: "IX_wagers_prediction_id",
                table: "wagers");

            migrationBuilder.DropIndex(
                name: "IX_prediction_options_prediction_id",
                table: "prediction_options");
        }
    }
}
