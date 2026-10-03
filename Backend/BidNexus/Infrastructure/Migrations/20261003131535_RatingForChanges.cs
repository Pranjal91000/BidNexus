using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RatingForChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingFor",
                schema: "GlobalData",
                table: "RatingParameter");

            migrationBuilder.AddColumn<short>(
                name: "RatingForId",
                schema: "GlobalData",
                table: "RatingParameter",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_RatingParameter_RatingForId",
                schema: "GlobalData",
                table: "RatingParameter",
                column: "RatingForId");

            migrationBuilder.AddForeignKey(
                name: "FK_RatingParameter_RatingFor_RatingForId",
                schema: "GlobalData",
                table: "RatingParameter",
                column: "RatingForId",
                principalSchema: "GlobalData",
                principalTable: "RatingFor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RatingParameter_RatingFor_RatingForId",
                schema: "GlobalData",
                table: "RatingParameter");

            migrationBuilder.DropIndex(
                name: "IX_RatingParameter_RatingForId",
                schema: "GlobalData",
                table: "RatingParameter");

            migrationBuilder.DropColumn(
                name: "RatingForId",
                schema: "GlobalData",
                table: "RatingParameter");

            migrationBuilder.AddColumn<bool>(
                name: "RatingFor",
                schema: "GlobalData",
                table: "RatingParameter",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
