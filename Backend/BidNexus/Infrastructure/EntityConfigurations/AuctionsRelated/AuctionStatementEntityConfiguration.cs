using Core.Abstraction.Services;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities.Auction;

namespace Infrastructure.EntityConfigurations.AuctionsRelated
{
    public class AuctionStatementEntityConfiguration(IJwtHelperService jwtHelperService): IEntityTypeConfiguration<AuctionStatement>
    {
        private readonly IJwtHelperService _jwtHelper = jwtHelperService;

        public void Configure(EntityTypeBuilder<AuctionStatement> builder)
        {
            builder.ToTable("AuctionStatement", "AuctionRel");
            BaseEntityConfiguration.Configure(builder);
            builder.HasQueryFilter(x => x.TenantId == _jwtHelper.GetTenantId());
            builder.Property(x => x.AuctionId).IsRequired();
            builder.Property(x => x.BidId).IsRequired();
            builder.Property(x => x.VendorId).IsRequired();
            builder.Property(x => x.Rank).IsRequired();
            builder.Property(x => x.IsWinner).IsRequired();

            builder.HasOne(x => x.Auction)
                .WithOne(x => x.AuctionStatement)
                .HasForeignKey<AuctionStatement>(x => x.AuctionId)
                .HasConstraintName("FK_AuctionStatement_AuctionId");

            builder.HasOne(x => x.Bid)
                .WithOne()
                .HasForeignKey<AuctionStatement>(x => x.BidId)
                .HasConstraintName("FK_AuctionStatement_BidId");

            builder.HasOne(x => x.Vendor)
                .WithOne()
                .HasForeignKey<AuctionStatement>(x => x.VendorId)
                .HasConstraintName("FK_AuctionStatement_VendorId");
        }
    }
}
