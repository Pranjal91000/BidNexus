using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxCodeAndTaxMasterUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaxCode",
                schema: "AuctionRel",
                table: "BidTaxDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TaxMasters_TenantId_Code",
                table: "TaxMasters",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxMasters_TenantId_Name",
                table: "TaxMasters",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.DropForeignKey(
                name: "FK_TaxMasters_ChargeType_ChargeTypeId",
                table: "TaxMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxMasters_Status_StatusId",
                table: "TaxMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxMasters_TaxNature_TaxNatureId",
                table: "TaxMasters");

            migrationBuilder.AddForeignKey(
                name: "FK_TaxMasters_ChargeType_ChargeTypeId",
                table: "TaxMasters",
                column: "ChargeTypeId",
                principalSchema: "GlobalData",
                principalTable: "ChargeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxMasters_Status_StatusId",
                table: "TaxMasters",
                column: "StatusId",
                principalSchema: "GlobalData",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxMasters_TaxNature_TaxNatureId",
                table: "TaxMasters",
                column: "TaxNatureId",
                principalSchema: "GlobalData",
                principalTable: "TaxNature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaxMasters_ChargeType_ChargeTypeId",
                table: "TaxMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxMasters_Status_StatusId",
                table: "TaxMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxMasters_TaxNature_TaxNatureId",
                table: "TaxMasters");

            migrationBuilder.DropIndex(
                name: "IX_TaxMasters_TenantId_Code",
                table: "TaxMasters");

            migrationBuilder.DropIndex(
                name: "IX_TaxMasters_TenantId_Name",
                table: "TaxMasters");

            migrationBuilder.DropColumn(
                name: "TaxCode",
                schema: "AuctionRel",
                table: "BidTaxDetail");

            migrationBuilder.AddForeignKey(
                name: "FK_TaxMasters_ChargeType_ChargeTypeId",
                table: "TaxMasters",
                column: "ChargeTypeId",
                principalSchema: "GlobalData",
                principalTable: "ChargeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxMasters_Status_StatusId",
                table: "TaxMasters",
                column: "StatusId",
                principalSchema: "GlobalData",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxMasters_TaxNature_TaxNatureId",
                table: "TaxMasters",
                column: "TaxNatureId",
                principalSchema: "GlobalData",
                principalTable: "TaxNature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
