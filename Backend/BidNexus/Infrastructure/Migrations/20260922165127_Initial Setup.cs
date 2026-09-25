using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AuctionRel");

            migrationBuilder.EnsureSchema(
                name: "GlobalData");

            migrationBuilder.EnsureSchema(
                name: "Master");

            migrationBuilder.EnsureSchema(
                name: "Auth");

            migrationBuilder.EnsureSchema(
                name: "Tenant");

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
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<int>(type: "integer", nullable: false),
                    ContactNumber = table.Column<int>(type: "integer", nullable: false),
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
                name: "Unit",
                schema: "Master",
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
                    StatusRemarks = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                schema: "Master",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<short>(type: "smallint", nullable: false),
                    ItemDescription = table.Column<string>(type: "text", nullable: true),
                    DocAttachmentId = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    StatusRemarks = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "GlobalData",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "TaxMaster",
                schema: "Master",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaxNatureId = table.Column<short>(type: "smallint", nullable: false),
                    ChargeTypeId = table.Column<short>(type: "smallint", nullable: false),
                    TaxValue = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    StatusRemarks = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxMaster_ChargeTypeId",
                        column: x => x.ChargeTypeId,
                        principalSchema: "GlobalData",
                        principalTable: "ChargeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaxMaster_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "GlobalData",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaxMaster_TaxNatureId",
                        column: x => x.TaxNatureId,
                        principalSchema: "GlobalData",
                        principalTable: "TaxNature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                schema: "Tenant",
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
                        principalSchema: "Tenant",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vendor",
                schema: "Tenant",
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
                        principalSchema: "Tenant",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemUnitMapping",
                schema: "Master",
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
                    table.PrimaryKey("PK_ItemUnitMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemUnitMapping_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Master",
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemUnitMapping_UnitId",
                        column: x => x.UnitId,
                        principalSchema: "Master",
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Auction",
                schema: "AuctionRel",
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
                    table.PrimaryKey("PK_Auction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auction_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Tenant",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Auction_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "GlobalData",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuctionRequirement",
                schema: "AuctionRel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuctionId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    TechnicalSpecification = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: false),
                    DocumentAttachmentId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionRequirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionRequirement_AuctioId",
                        column: x => x.AuctionId,
                        principalSchema: "AuctionRel",
                        principalTable: "Auction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionRequirement_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Master",
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuctionRequirement_UnitId",
                        column: x => x.UnitId,
                        principalSchema: "Master",
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        principalSchema: "AuctionRel",
                        principalTable: "Auction",
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
                        principalSchema: "Tenant",
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
                        principalSchema: "Tenant",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rating_AuctionId",
                        column: x => x.AuctionId,
                        principalSchema: "AuctionRel",
                        principalTable: "Auction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rating_SubmittedByTenant",
                        column: x => x.SubmittedByTenant,
                        principalSchema: "Tenant",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorIntent",
                schema: "AuctionRel",
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
                    table.PrimaryKey("PK_VendorIntent", x => x.Id);
                    table.ForeignKey(
                        name: "Fk_VendorIntent_AuctionId",
                        column: x => x.AuctionId,
                        principalSchema: "AuctionRel",
                        principalTable: "Auction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Fk_VendorIntent_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "Tenant",
                        principalTable: "Vendor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuctionStatement",
                schema: "AuctionRel",
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
                    table.PrimaryKey("PK_AuctionStatement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionStatement_AuctionId",
                        column: x => x.AuctionId,
                        principalSchema: "AuctionRel",
                        principalTable: "Auction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionStatement_BidId",
                        column: x => x.BidId,
                        principalSchema: "AuctionRel",
                        principalTable: "Bid",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionStatement_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "Tenant",
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
                        principalSchema: "AuctionRel",
                        principalTable: "AuctionRequirement",
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
                        principalSchema: "Master",
                        principalTable: "TaxMaster",
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
                name: "IX_Auction_OrganizationId",
                schema: "AuctionRel",
                table: "Auction",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Auction_StatusId",
                schema: "AuctionRel",
                table: "Auction",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionRequirement_AuctionId",
                schema: "AuctionRel",
                table: "AuctionRequirement",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionRequirement_ItemId",
                schema: "AuctionRel",
                table: "AuctionRequirement",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionRequirement_UnitId",
                schema: "AuctionRel",
                table: "AuctionRequirement",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionStatement_AuctionId",
                schema: "AuctionRel",
                table: "AuctionStatement",
                column: "AuctionId",
                unique: true);

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
                name: "IX_Item_CategoryId",
                schema: "Master",
                table: "Item",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemUnitMapping_ItemId",
                schema: "Master",
                table: "ItemUnitMapping",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemUnitMapping_UnitId",
                schema: "Master",
                table: "ItemUnitMapping",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_StatusId",
                schema: "Auth",
                table: "LoginAttempt",
                column: "StatusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_TenantId",
                schema: "Tenant",
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
                name: "IX_TaxMaster_ChargeTypeId",
                schema: "Master",
                table: "TaxMaster",
                column: "ChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxMaster_StatusId",
                schema: "Master",
                table: "TaxMaster",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxMaster_TaxNatureId",
                schema: "Master",
                table: "TaxMaster",
                column: "TaxNatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_EmailAddress",
                schema: "Tenant",
                table: "Tenant",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_UserName",
                schema: "Tenant",
                table: "Tenant",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendor_TenantId",
                schema: "Tenant",
                table: "Vendor",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorIntent_AuctionId",
                schema: "AuctionRel",
                table: "VendorIntent",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorIntent_VendorId",
                schema: "AuctionRel",
                table: "VendorIntent",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionStatement",
                schema: "AuctionRel");

            migrationBuilder.DropTable(
                name: "BidTaxDetail",
                schema: "AuctionRel");

            migrationBuilder.DropTable(
                name: "ItemUnitMapping",
                schema: "Master");

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
                name: "VendorIntent",
                schema: "AuctionRel");

            migrationBuilder.DropTable(
                name: "BidDetail",
                schema: "AuctionRel");

            migrationBuilder.DropTable(
                name: "TaxMaster",
                schema: "Master");

            migrationBuilder.DropTable(
                name: "RatingParameter",
                schema: "GlobalData");

            migrationBuilder.DropTable(
                name: "Rating",
                schema: "Utilities");

            migrationBuilder.DropTable(
                name: "AuctionRequirement",
                schema: "AuctionRel");

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
                name: "Item",
                schema: "Master");

            migrationBuilder.DropTable(
                name: "Unit",
                schema: "Master");

            migrationBuilder.DropTable(
                name: "Auction",
                schema: "AuctionRel");

            migrationBuilder.DropTable(
                name: "Vendor",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "GlobalData");

            migrationBuilder.DropTable(
                name: "Organization",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "Status",
                schema: "GlobalData");

            migrationBuilder.DropTable(
                name: "Tenant",
                schema: "Tenant");
        }
    }
}
