using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AuctionRel");

            migrationBuilder.EnsureSchema(
                name: "GlobalData");

            migrationBuilder.EnsureSchema(
                name: "Auth");

            migrationBuilder.EnsureSchema(
                name: "TenantRel");

            migrationBuilder.EnsureSchema(
                name: "Utilities");

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "GlobalData",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargeType",
                schema: "GlobalData",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RatingFor",
                schema: "GlobalData",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    For = table.Column<string>(type: "text", nullable: false),
                    Inactive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingFor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RatingParameter",
                schema: "GlobalData",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParameterName = table.Column<string>(type: "text", nullable: false),
                    RatingFor = table.Column<bool>(type: "boolean", nullable: false),
                    Inactive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingParameter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                schema: "GlobalData",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Inactive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxNature",
                schema: "GlobalData",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxNature", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                schema: "TenantRel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContactNumber = table.Column<string>(type: "text", nullable: false),
                    EmailAddress = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: false),
                    IsVendor = table.Column<bool>(type: "boolean", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    StatusRemarks = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<short>(type: "smallint", nullable: false),
                    ItemDescription = table.Column<string>(type: "text", nullable: false),
                    DocAttachmentId = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    StatusRemarks = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "GlobalData",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoginAttempt",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FailedLoginAttemptCount = table.Column<short>(type: "smallint", nullable: false),
                    LastAttemptedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAttempt", x => x.Id);
                    table.ForeignKey(
                        name: "Fk_LoginAttempt_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "GlobalData",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaxMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaxNatureId = table.Column<short>(type: "smallint", nullable: false),
                    ChargeTypeId = table.Column<short>(type: "smallint", nullable: false),
                    TaxValue = table.Column<decimal>(type: "numeric", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    StatusRemarks = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxMasters_ChargeType_ChargeTypeId",
                        column: x => x.ChargeTypeId,
                        principalSchema: "GlobalData",
                        principalTable: "ChargeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaxMasters_Status_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "GlobalData",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaxMasters_TaxNature_TaxNatureId",
                        column: x => x.TaxNatureId,
                        principalSchema: "GlobalData",
                        principalTable: "TaxNature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                schema: "TenantRel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OfficialAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ForegroundImageId = table.Column<int>(type: "integer", nullable: true),
                    About = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organization_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "TenantRel",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vendor",
                schema: "TenantRel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ForegroundImageId = table.Column<int>(type: "integer", nullable: true),
                    About = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organization_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "TenantRel",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemUnitMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemUnitMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemUnitMappings_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsForwardAuction = table.Column<bool>(type: "boolean", nullable: false),
                    AuctionStartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AuctionEndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DocAttachmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    OpenToAll = table.Column<bool>(type: "boolean", nullable: false),
                    IsBidPriceHidden = table.Column<bool>(type: "boolean", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    DocNoYearly = table.Column<string>(type: "text", nullable: false),
                    DocDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auctions_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "TenantRel",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Auctions_Status_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "GlobalData",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuctionRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LineNo = table.Column<short>(type: "smallint", nullable: false),
                    AuctionId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    TechnicalSpecification = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: false),
                    DocumentAttachmentId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionRequirements_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionRequirements_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionRequirements_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bid",
                schema: "AuctionRel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    MainBidId = table.Column<long>(type: "bigint", nullable: false),
                    AuctionId = table.Column<int>(type: "integer", nullable: false),
                    VendorId = table.Column<int>(type: "integer", nullable: false),
                    BasicAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BidRevisionNo = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bid", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bid_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bid_MainBidId",
                        column: x => x.MainBidId,
                        principalSchema: "AuctionRel",
                        principalTable: "Bid",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bid_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "TenantRel",
                        principalTable: "Vendor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                schema: "Utilities",
                columns: table => new
                {
                    RatingForId = table.Column<short>(type: "smallint", nullable: false),
                    AuctionId = table.Column<int>(type: "integer", nullable: false),
                    AgainstTenant = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByTenant = table.Column<int>(type: "integer", nullable: false),
                    Remark = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => new { x.RatingForId, x.AuctionId, x.AgainstTenant, x.SubmittedByTenant });
                    table.ForeignKey(
                        name: "FK_Rating_AgainstTenant",
                        column: x => x.AgainstTenant,
                        principalSchema: "TenantRel",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rating_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rating_SubmittedByTenant",
                        column: x => x.SubmittedByTenant,
                        principalSchema: "TenantRel",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorIntents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuctionId = table.Column<int>(type: "integer", nullable: false),
                    VendorId = table.Column<int>(type: "integer", nullable: false),
                    IsInterested = table.Column<bool>(type: "boolean", nullable: false),
                    IsQualified = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorIntents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorIntents_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendorIntents_Vendor_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "TenantRel",
                        principalTable: "Vendor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuctionStatements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuctionId = table.Column<int>(type: "integer", nullable: false),
                    BidId = table.Column<long>(type: "bigint", nullable: false),
                    VendorId = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<short>(type: "smallint", nullable: false),
                    IsWinner = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionStatements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionStatements_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionStatements_Bid_BidId",
                        column: x => x.BidId,
                        principalSchema: "AuctionRel",
                        principalTable: "Bid",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionStatements_Vendor_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "TenantRel",
                        principalTable: "Vendor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BidDetail",
                schema: "AuctionRel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BidId = table.Column<long>(type: "bigint", nullable: false),
                    AuctionRequirementId = table.Column<int>(type: "integer", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    BaseAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BidDetail_AuctionRequirementId",
                        column: x => x.AuctionRequirementId,
                        principalTable: "AuctionRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BidDetail_BidId",
                        column: x => x.BidId,
                        principalSchema: "AuctionRel",
                        principalTable: "Bid",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RatingValue",
                schema: "Utilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RatingParameterId = table.Column<short>(type: "smallint", nullable: false),
                    RatingScore = table.Column<short>(type: "smallint", nullable: false),
                    RatingAgainstTenant = table.Column<int>(type: "integer", nullable: true),
                    RatingAuctionId = table.Column<int>(type: "integer", nullable: true),
                    RatingForId = table.Column<short>(type: "smallint", nullable: true),
                    RatingSubmittedByTenant = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatingValue_RatingParameterId",
                        column: x => x.RatingParameterId,
                        principalSchema: "GlobalData",
                        principalTable: "RatingParameter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingValue_Rating_RatingForId_RatingAuctionId_RatingAgains~",
                        columns: x => new { x.RatingForId, x.RatingAuctionId, x.RatingAgainstTenant, x.RatingSubmittedByTenant },
                        principalSchema: "Utilities",
                        principalTable: "Rating",
                        principalColumns: new[] { "RatingForId", "AuctionId", "AgainstTenant", "SubmittedByTenant" });
                });

            migrationBuilder.CreateTable(
                name: "BidTaxDetail",
                schema: "AuctionRel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BidDetailId = table.Column<long>(type: "bigint", nullable: false),
                    TaxId = table.Column<int>(type: "integer", nullable: true),
                    TaxName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TaxCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TaxNatureId = table.Column<short>(type: "smallint", nullable: false),
                    ChargeTypeId = table.Column<short>(type: "smallint", nullable: false),
                    TaxValue = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidTaxDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BidTaxDetail_BidDetailId",
                        column: x => x.BidDetailId,
                        principalSchema: "AuctionRel",
                        principalTable: "BidDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BidTaxDetail_ChargeTypeId",
                        column: x => x.ChargeTypeId,
                        principalSchema: "GlobalData",
                        principalTable: "ChargeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BidTaxDetail_TaxId",
                        column: x => x.TaxId,
                        principalTable: "TaxMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BidTaxDetail_TaxNatureId",
                        column: x => x.TaxNatureId,
                        principalSchema: "GlobalData",
                        principalTable: "TaxNature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuctionRequirements_AuctionId",
                table: "AuctionRequirements",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionRequirements_ItemId",
                table: "AuctionRequirements",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionRequirements_UnitId",
                table: "AuctionRequirements",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_OrganizationId",
                table: "Auctions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_StatusId",
                table: "Auctions",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionStatements_AuctionId",
                table: "AuctionStatements",
                column: "AuctionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuctionStatements_BidId",
                table: "AuctionStatements",
                column: "BidId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionStatements_VendorId",
                table: "AuctionStatements",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Bid_AuctionId",
                schema: "AuctionRel",
                table: "Bid",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_Bid_MainBidId",
                schema: "AuctionRel",
                table: "Bid",
                column: "MainBidId");

            migrationBuilder.CreateIndex(
                name: "IX_Bid_VendorId",
                schema: "AuctionRel",
                table: "Bid",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_BidDetail_AuctionRequirementId",
                schema: "AuctionRel",
                table: "BidDetail",
                column: "AuctionRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_BidDetail_BidId",
                schema: "AuctionRel",
                table: "BidDetail",
                column: "BidId");

            migrationBuilder.CreateIndex(
                name: "IX_BidTaxDetail_BidDetailId",
                schema: "AuctionRel",
                table: "BidTaxDetail",
                column: "BidDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_BidTaxDetail_ChargeTypeId",
                schema: "AuctionRel",
                table: "BidTaxDetail",
                column: "ChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BidTaxDetail_TaxId",
                schema: "AuctionRel",
                table: "BidTaxDetail",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_BidTaxDetail_TaxNatureId",
                schema: "AuctionRel",
                table: "BidTaxDetail",
                column: "TaxNatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryId",
                table: "Items",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemUnitMappings_ItemId",
                table: "ItemUnitMappings",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_StatusId",
                schema: "Auth",
                table: "LoginAttempt",
                column: "StatusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_TenantId",
                schema: "TenantRel",
                table: "Organization",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_AgainstTenant",
                schema: "Utilities",
                table: "Rating",
                column: "AgainstTenant");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_AuctionId",
                schema: "Utilities",
                table: "Rating",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_SubmittedByTenant",
                schema: "Utilities",
                table: "Rating",
                column: "SubmittedByTenant");

            migrationBuilder.CreateIndex(
                name: "IX_RatingValue_RatingForId_RatingAuctionId_RatingAgainstTenant~",
                schema: "Utilities",
                table: "RatingValue",
                columns: new[] { "RatingForId", "RatingAuctionId", "RatingAgainstTenant", "RatingSubmittedByTenant" });

            migrationBuilder.CreateIndex(
                name: "IX_RatingValue_RatingParameterId",
                schema: "Utilities",
                table: "RatingValue",
                column: "RatingParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxMasters_ChargeTypeId",
                table: "TaxMasters",
                column: "ChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxMasters_StatusId",
                table: "TaxMasters",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxMasters_TaxNatureId",
                table: "TaxMasters",
                column: "TaxNatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_EmailAddress",
                schema: "TenantRel",
                table: "Tenant",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_UserName",
                schema: "TenantRel",
                table: "Tenant",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendor_TenantId",
                schema: "TenantRel",
                table: "Vendor",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorIntents_AuctionId",
                table: "VendorIntents",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorIntents_VendorId",
                table: "VendorIntents",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionStatements");

            migrationBuilder.DropTable(
                name: "BidTaxDetail",
                schema: "AuctionRel");

            migrationBuilder.DropTable(
                name: "ItemUnitMappings");

            migrationBuilder.DropTable(
                name: "LoginAttempt",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "RatingFor",
                schema: "GlobalData");

            migrationBuilder.DropTable(
                name: "RatingValue",
                schema: "Utilities");

            migrationBuilder.DropTable(
                name: "VendorIntents");

            migrationBuilder.DropTable(
                name: "BidDetail",
                schema: "AuctionRel");

            migrationBuilder.DropTable(
                name: "TaxMasters");

            migrationBuilder.DropTable(
                name: "RatingParameter",
                schema: "GlobalData");

            migrationBuilder.DropTable(
                name: "Rating",
                schema: "Utilities");

            migrationBuilder.DropTable(
                name: "AuctionRequirements");

            migrationBuilder.DropTable(
                name: "Bid",
                schema: "AuctionRel");

            migrationBuilder.DropTable(
                name: "ChargeType",
                schema: "GlobalData");

            migrationBuilder.DropTable(
                name: "TaxNature",
                schema: "GlobalData");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Auctions");

            migrationBuilder.DropTable(
                name: "Vendor",
                schema: "TenantRel");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "GlobalData");

            migrationBuilder.DropTable(
                name: "Organization",
                schema: "TenantRel");

            migrationBuilder.DropTable(
                name: "Status",
                schema: "GlobalData");

            migrationBuilder.DropTable(
                name: "Tenant",
                schema: "TenantRel");
        }
    }
}
