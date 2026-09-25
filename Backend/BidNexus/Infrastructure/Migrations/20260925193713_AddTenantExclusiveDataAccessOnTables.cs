using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantExclusiveDataAccessOnTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auction_OrganizationId",
                schema: "AuctionRel",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_Auction_StatusId",
                schema: "AuctionRel",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionRequirement_AuctioId",
                schema: "AuctionRel",
                table: "AuctionRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionRequirement_ItemId",
                schema: "AuctionRel",
                table: "AuctionRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionRequirement_UnitId",
                schema: "AuctionRel",
                table: "AuctionRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionStatement_AuctionId",
                schema: "AuctionRel",
                table: "AuctionStatement");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionStatement_BidId",
                schema: "AuctionRel",
                table: "AuctionStatement");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionStatement_VendorId",
                schema: "AuctionRel",
                table: "AuctionStatement");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_CategoryId",
                schema: "Master",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemUnitMapping_ItemId",
                schema: "Master",
                table: "ItemUnitMapping");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemUnitMapping_UnitId",
                schema: "Master",
                table: "ItemUnitMapping");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxMaster_ChargeTypeId",
                schema: "Master",
                table: "TaxMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxMaster_StatusId",
                schema: "Master",
                table: "TaxMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxMaster_TaxNatureId",
                schema: "Master",
                table: "TaxMaster");

            migrationBuilder.DropForeignKey(
                name: "Fk_VendorIntent_AuctionId",
                schema: "AuctionRel",
                table: "VendorIntent");

            migrationBuilder.DropForeignKey(
                name: "Fk_VendorIntent_VendorId",
                schema: "AuctionRel",
                table: "VendorIntent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VendorIntent",
                schema: "AuctionRel",
                table: "VendorIntent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Unit",
                schema: "Master",
                table: "Unit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaxMaster",
                schema: "Master",
                table: "TaxMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemUnitMapping",
                schema: "Master",
                table: "ItemUnitMapping");

            migrationBuilder.DropIndex(
                name: "IX_ItemUnitMapping_UnitId",
                schema: "Master",
                table: "ItemUnitMapping");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Item",
                schema: "Master",
                table: "Item");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuctionStatement",
                schema: "AuctionRel",
                table: "AuctionStatement");

            migrationBuilder.DropIndex(
                name: "IX_AuctionStatement_BidId",
                schema: "AuctionRel",
                table: "AuctionStatement");

            migrationBuilder.DropIndex(
                name: "IX_AuctionStatement_VendorId",
                schema: "AuctionRel",
                table: "AuctionStatement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuctionRequirement",
                schema: "AuctionRel",
                table: "AuctionRequirement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Auction",
                schema: "AuctionRel",
                table: "Auction");

            migrationBuilder.RenameTable(
                name: "VendorIntent",
                schema: "AuctionRel",
                newName: "VendorIntents");

            migrationBuilder.RenameTable(
                name: "Unit",
                schema: "Master",
                newName: "Units");

            migrationBuilder.RenameTable(
                name: "TaxMaster",
                schema: "Master",
                newName: "TaxMasters");

            migrationBuilder.RenameTable(
                name: "ItemUnitMapping",
                schema: "Master",
                newName: "ItemUnitMappings");

            migrationBuilder.RenameTable(
                name: "Item",
                schema: "Master",
                newName: "Items");

            migrationBuilder.RenameTable(
                name: "AuctionStatement",
                schema: "AuctionRel",
                newName: "AuctionStatements");

            migrationBuilder.RenameTable(
                name: "AuctionRequirement",
                schema: "AuctionRel",
                newName: "AuctionRequirements");

            migrationBuilder.RenameTable(
                name: "Auction",
                schema: "AuctionRel",
                newName: "Auctions");

            migrationBuilder.RenameIndex(
                name: "IX_VendorIntent_VendorId",
                table: "VendorIntents",
                newName: "IX_VendorIntents_VendorId");

            migrationBuilder.RenameIndex(
                name: "IX_VendorIntent_AuctionId",
                table: "VendorIntents",
                newName: "IX_VendorIntents_AuctionId");

            migrationBuilder.RenameIndex(
                name: "IX_TaxMaster_TaxNatureId",
                table: "TaxMasters",
                newName: "IX_TaxMasters_TaxNatureId");

            migrationBuilder.RenameIndex(
                name: "IX_TaxMaster_StatusId",
                table: "TaxMasters",
                newName: "IX_TaxMasters_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_TaxMaster_ChargeTypeId",
                table: "TaxMasters",
                newName: "IX_TaxMasters_ChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemUnitMapping_ItemId",
                table: "ItemUnitMappings",
                newName: "IX_ItemUnitMappings_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Item_CategoryId",
                table: "Items",
                newName: "IX_Items_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_AuctionStatement_AuctionId",
                table: "AuctionStatements",
                newName: "IX_AuctionStatements_AuctionId");

            migrationBuilder.RenameIndex(
                name: "IX_AuctionRequirement_UnitId",
                table: "AuctionRequirements",
                newName: "IX_AuctionRequirements_UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_AuctionRequirement_ItemId",
                table: "AuctionRequirements",
                newName: "IX_AuctionRequirements_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_AuctionRequirement_AuctionId",
                table: "AuctionRequirements",
                newName: "IX_AuctionRequirements_AuctionId");

            migrationBuilder.RenameIndex(
                name: "IX_Auction_StatusId",
                table: "Auctions",
                newName: "IX_Auctions_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Auction_OrganizationId",
                table: "Auctions",
                newName: "IX_Auctions_OrganizationId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Tenant",
                table: "Tenant",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ContactNumber",
                schema: "Tenant",
                table: "Tenant",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "StatusRemarks",
                table: "Units",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxValue",
                table: "TaxMasters",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<string>(
                name: "StatusRemarks",
                table: "TaxMasters",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatusRemarks",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemDescription",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "AuctionRequirements",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VendorIntents",
                table: "VendorIntents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Units",
                table: "Units",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaxMasters",
                table: "TaxMasters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemUnitMappings",
                table: "ItemUnitMappings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuctionStatements",
                table: "AuctionStatements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuctionRequirements",
                table: "AuctionRequirements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Auctions",
                table: "Auctions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionStatements_BidId",
                table: "AuctionStatements",
                column: "BidId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionStatements_VendorId",
                table: "AuctionStatements",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionRequirements_Auctions_AuctionId",
                table: "AuctionRequirements",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionRequirements_Items_ItemId",
                table: "AuctionRequirements",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionRequirements_Units_UnitId",
                table: "AuctionRequirements",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Organization_OrganizationId",
                table: "Auctions",
                column: "OrganizationId",
                principalSchema: "Tenant",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Status_StatusId",
                table: "Auctions",
                column: "StatusId",
                principalSchema: "GlobalData",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionStatements_Auctions_AuctionId",
                table: "AuctionStatements",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionStatements_Bid_BidId",
                table: "AuctionStatements",
                column: "BidId",
                principalSchema: "AuctionRel",
                principalTable: "Bid",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionStatements_Vendor_VendorId",
                table: "AuctionStatements",
                column: "VendorId",
                principalSchema: "Tenant",
                principalTable: "Vendor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Category_CategoryId",
                table: "Items",
                column: "CategoryId",
                principalSchema: "GlobalData",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemUnitMappings_Items_ItemId",
                table: "ItemUnitMappings",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_VendorIntents_Auctions_AuctionId",
                table: "VendorIntents",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorIntents_Vendor_VendorId",
                table: "VendorIntents",
                column: "VendorId",
                principalSchema: "Tenant",
                principalTable: "Vendor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionRequirements_Auctions_AuctionId",
                table: "AuctionRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionRequirements_Items_ItemId",
                table: "AuctionRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionRequirements_Units_UnitId",
                table: "AuctionRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Organization_OrganizationId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Status_StatusId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionStatements_Auctions_AuctionId",
                table: "AuctionStatements");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionStatements_Bid_BidId",
                table: "AuctionStatements");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionStatements_Vendor_VendorId",
                table: "AuctionStatements");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Category_CategoryId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemUnitMappings_Items_ItemId",
                table: "ItemUnitMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxMasters_ChargeType_ChargeTypeId",
                table: "TaxMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxMasters_Status_StatusId",
                table: "TaxMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxMasters_TaxNature_TaxNatureId",
                table: "TaxMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorIntents_Auctions_AuctionId",
                table: "VendorIntents");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorIntents_Vendor_VendorId",
                table: "VendorIntents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VendorIntents",
                table: "VendorIntents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Units",
                table: "Units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaxMasters",
                table: "TaxMasters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemUnitMappings",
                table: "ItemUnitMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuctionStatements",
                table: "AuctionStatements");

            migrationBuilder.DropIndex(
                name: "IX_AuctionStatements_BidId",
                table: "AuctionStatements");

            migrationBuilder.DropIndex(
                name: "IX_AuctionStatements_VendorId",
                table: "AuctionStatements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Auctions",
                table: "Auctions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuctionRequirements",
                table: "AuctionRequirements");

            migrationBuilder.EnsureSchema(
                name: "Master");

            migrationBuilder.RenameTable(
                name: "VendorIntents",
                newName: "VendorIntent",
                newSchema: "AuctionRel");

            migrationBuilder.RenameTable(
                name: "Units",
                newName: "Unit",
                newSchema: "Master");

            migrationBuilder.RenameTable(
                name: "TaxMasters",
                newName: "TaxMaster",
                newSchema: "Master");

            migrationBuilder.RenameTable(
                name: "ItemUnitMappings",
                newName: "ItemUnitMapping",
                newSchema: "Master");

            migrationBuilder.RenameTable(
                name: "Items",
                newName: "Item",
                newSchema: "Master");

            migrationBuilder.RenameTable(
                name: "AuctionStatements",
                newName: "AuctionStatement",
                newSchema: "AuctionRel");

            migrationBuilder.RenameTable(
                name: "Auctions",
                newName: "Auction",
                newSchema: "AuctionRel");

            migrationBuilder.RenameTable(
                name: "AuctionRequirements",
                newName: "AuctionRequirement",
                newSchema: "AuctionRel");

            migrationBuilder.RenameIndex(
                name: "IX_VendorIntents_VendorId",
                schema: "AuctionRel",
                table: "VendorIntent",
                newName: "IX_VendorIntent_VendorId");

            migrationBuilder.RenameIndex(
                name: "IX_VendorIntents_AuctionId",
                schema: "AuctionRel",
                table: "VendorIntent",
                newName: "IX_VendorIntent_AuctionId");

            migrationBuilder.RenameIndex(
                name: "IX_TaxMasters_TaxNatureId",
                schema: "Master",
                table: "TaxMaster",
                newName: "IX_TaxMaster_TaxNatureId");

            migrationBuilder.RenameIndex(
                name: "IX_TaxMasters_StatusId",
                schema: "Master",
                table: "TaxMaster",
                newName: "IX_TaxMaster_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_TaxMasters_ChargeTypeId",
                schema: "Master",
                table: "TaxMaster",
                newName: "IX_TaxMaster_ChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemUnitMappings_ItemId",
                schema: "Master",
                table: "ItemUnitMapping",
                newName: "IX_ItemUnitMapping_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Items_CategoryId",
                schema: "Master",
                table: "Item",
                newName: "IX_Item_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_AuctionStatements_AuctionId",
                schema: "AuctionRel",
                table: "AuctionStatement",
                newName: "IX_AuctionStatement_AuctionId");

            migrationBuilder.RenameIndex(
                name: "IX_Auctions_StatusId",
                schema: "AuctionRel",
                table: "Auction",
                newName: "IX_Auction_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Auctions_OrganizationId",
                schema: "AuctionRel",
                table: "Auction",
                newName: "IX_Auction_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_AuctionRequirements_UnitId",
                schema: "AuctionRel",
                table: "AuctionRequirement",
                newName: "IX_AuctionRequirement_UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_AuctionRequirements_ItemId",
                schema: "AuctionRel",
                table: "AuctionRequirement",
                newName: "IX_AuctionRequirement_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_AuctionRequirements_AuctionId",
                schema: "AuctionRel",
                table: "AuctionRequirement",
                newName: "IX_AuctionRequirement_AuctionId");

            migrationBuilder.AlterColumn<int>(
                name: "Name",
                schema: "Tenant",
                table: "Tenant",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "ContactNumber",
                schema: "Tenant",
                table: "Tenant",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "StatusRemarks",
                schema: "Master",
                table: "Unit",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxValue",
                schema: "Master",
                table: "TaxMaster",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "StatusRemarks",
                schema: "Master",
                table: "TaxMaster",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "StatusRemarks",
                schema: "Master",
                table: "Item",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ItemDescription",
                schema: "Master",
                table: "Item",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                schema: "AuctionRel",
                table: "AuctionRequirement",
                type: "numeric(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VendorIntent",
                schema: "AuctionRel",
                table: "VendorIntent",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Unit",
                schema: "Master",
                table: "Unit",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaxMaster",
                schema: "Master",
                table: "TaxMaster",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemUnitMapping",
                schema: "Master",
                table: "ItemUnitMapping",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Item",
                schema: "Master",
                table: "Item",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuctionStatement",
                schema: "AuctionRel",
                table: "AuctionStatement",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Auction",
                schema: "AuctionRel",
                table: "Auction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuctionRequirement",
                schema: "AuctionRel",
                table: "AuctionRequirement",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ItemUnitMapping_UnitId",
                schema: "Master",
                table: "ItemUnitMapping",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionStatement_BidId",
                schema: "AuctionRel",
                table: "AuctionStatement",
                column: "BidId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuctionStatement_VendorId",
                schema: "AuctionRel",
                table: "AuctionStatement",
                column: "VendorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_OrganizationId",
                schema: "AuctionRel",
                table: "Auction",
                column: "OrganizationId",
                principalSchema: "Tenant",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_StatusId",
                schema: "AuctionRel",
                table: "Auction",
                column: "StatusId",
                principalSchema: "GlobalData",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionRequirement_AuctioId",
                schema: "AuctionRel",
                table: "AuctionRequirement",
                column: "AuctionId",
                principalSchema: "AuctionRel",
                principalTable: "Auction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionRequirement_ItemId",
                schema: "AuctionRel",
                table: "AuctionRequirement",
                column: "ItemId",
                principalSchema: "Master",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionRequirement_UnitId",
                schema: "AuctionRel",
                table: "AuctionRequirement",
                column: "UnitId",
                principalSchema: "Master",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionStatement_AuctionId",
                schema: "AuctionRel",
                table: "AuctionStatement",
                column: "AuctionId",
                principalSchema: "AuctionRel",
                principalTable: "Auction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionStatement_BidId",
                schema: "AuctionRel",
                table: "AuctionStatement",
                column: "BidId",
                principalSchema: "AuctionRel",
                principalTable: "Bid",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionStatement_VendorId",
                schema: "AuctionRel",
                table: "AuctionStatement",
                column: "VendorId",
                principalSchema: "Tenant",
                principalTable: "Vendor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_CategoryId",
                schema: "Master",
                table: "Item",
                column: "CategoryId",
                principalSchema: "GlobalData",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemUnitMapping_ItemId",
                schema: "Master",
                table: "ItemUnitMapping",
                column: "ItemId",
                principalSchema: "Master",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemUnitMapping_UnitId",
                schema: "Master",
                table: "ItemUnitMapping",
                column: "UnitId",
                principalSchema: "Master",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxMaster_ChargeTypeId",
                schema: "Master",
                table: "TaxMaster",
                column: "ChargeTypeId",
                principalSchema: "GlobalData",
                principalTable: "ChargeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxMaster_StatusId",
                schema: "Master",
                table: "TaxMaster",
                column: "StatusId",
                principalSchema: "GlobalData",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxMaster_TaxNatureId",
                schema: "Master",
                table: "TaxMaster",
                column: "TaxNatureId",
                principalSchema: "GlobalData",
                principalTable: "TaxNature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "Fk_VendorIntent_AuctionId",
                schema: "AuctionRel",
                table: "VendorIntent",
                column: "AuctionId",
                principalSchema: "AuctionRel",
                principalTable: "Auction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "Fk_VendorIntent_VendorId",
                schema: "AuctionRel",
                table: "VendorIntent",
                column: "VendorId",
                principalSchema: "Tenant",
                principalTable: "Vendor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
