using Core.Entities.Auction;
using Core.Entities.GlobalData;
using Core.Entities.Master;
using Core.Entities.Tenant;
using Core.Entities.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // Auction Related
    public DbSet<Auction> Auctions => Set<Auction>();
    public DbSet<AuctionRequirement> AuctionRequirements => Set<AuctionRequirement>();
    public DbSet<AuctionStatement> AuctionStatements => Set<AuctionStatement>();
    public DbSet<Bid> Bids => Set<Bid>();
    public DbSet<BidDetail> BidDetails => Set<BidDetail>();
    public DbSet<BidTaxDetail> BidTaxDetails => Set<BidTaxDetail>();
    public DbSet<VendorIntent> VendorIntents => Set<VendorIntent>();

    // Masters
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemUnitMapping> ItemUnitMappings => Set<ItemUnitMapping>();
    public DbSet<Unit> Units => Set<Unit>();

    // Tenant Related
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Vendor> Vendors => Set<Vendor>();

    // Utilities
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<RatingValue> RatingValues => Set<RatingValue>();

    // Global Data
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ChargeType> ChargeTypes => Set<ChargeType>();
    public DbSet<RatingFor> RatingFors => Set<RatingFor>();
    public DbSet<RatingParameter> RatingParameters => Set<RatingParameter>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<TaxNature> TaxNatures => Set<TaxNature>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}