using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bid_AuctionId",
                schema: "AuctionRel",
                table: "Bid");

            migrationBuilder.CreateIndex(
                name: "IX_Bid_AuctionId_VendorId_BidRevisionNo",
                schema: "AuctionRel",
                table: "Bid",
                columns: new[] { "AuctionId", "VendorId", "BidRevisionNo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bid_AuctionId_VendorId_BidRevisionNo",
                schema: "AuctionRel",
                table: "Bid");

            migrationBuilder.CreateIndex(
                name: "IX_Bid_AuctionId",
                schema: "AuctionRel",
                table: "Bid",
                column: "AuctionId");
        }
    }
}
