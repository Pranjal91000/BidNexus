using Core.Abstraction.Services;
using Core.Entities.Auction;
using Core.Entities.GlobalData;
using Core.Entities.Master;
using Core.Entities.TenantRelated;
using Core.Entities.Utilities;
using Infrastructure.EntityConfigurations.Master;
using Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IJwtHelperService? jwtHelperService = null) : DbContext(options)
{
    private readonly IJwtHelperService _jwtHelper = jwtHelperService ?? new JwtHelperService();

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
    public DbSet<TaxMaster> TaxMasters => Set<TaxMaster>();
    public DbSet<Unit> Units => Set<Unit>();

    // Tenant Related
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Vendor> Vendors => Set<Vendor>();

    // Utilities
    public DbSet<Attachment> Attachments => Set<Attachment>();
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

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Core.Entities.Shared.TransactionBaseEntity transactionEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    if (transactionEntity.CreatedDateTime == default)
                        transactionEntity.CreatedDateTime = now;
                    if (transactionEntity.LastModifiedDateTime == default)
                        transactionEntity.LastModifiedDateTime = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    transactionEntity.LastModifiedDateTime = now;
                }
            }
            else if (entry.Entity is Core.Entities.Shared.MasterBaseEntity masterEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    if (masterEntity.CreatedDateTime == default)
                        masterEntity.CreatedDateTime = now;
                    if (masterEntity.LastModifiedDateTime == default)
                        masterEntity.LastModifiedDateTime = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    masterEntity.LastModifiedDateTime = now;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}