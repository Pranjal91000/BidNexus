using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

public partial class FinalizeAuctionStatementCardinality : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_AuctionStatement_AuctionId",
            schema: "AuctionRel",
            table: "AuctionStatement");

        migrationBuilder.DropIndex(
            name: "IX_AuctionStatement_VendorId",
            schema: "AuctionRel",
            table: "AuctionStatement");

        migrationBuilder.CreateIndex(
            name: "IX_AuctionStatement_AuctionId_Rank",
            schema: "AuctionRel",
            table: "AuctionStatement",
            columns: new[] { "AuctionId", "Rank" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AuctionStatement_AuctionId_VendorId",
            schema: "AuctionRel",
            table: "AuctionStatement",
            columns: new[] { "AuctionId", "VendorId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_AuctionStatement_AuctionId_Rank",
            schema: "AuctionRel",
            table: "AuctionStatement");

        migrationBuilder.DropIndex(
            name: "IX_AuctionStatement_AuctionId_VendorId",
            schema: "AuctionRel",
            table: "AuctionStatement");

        migrationBuilder.CreateIndex(
            name: "IX_AuctionStatement_AuctionId",
            schema: "AuctionRel",
            table: "AuctionStatement",
            column: "AuctionId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AuctionStatement_VendorId",
            schema: "AuctionRel",
            table: "AuctionStatement",
            column: "VendorId",
            unique: true);
    }
}