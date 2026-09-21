using Core.Entities.Auction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.AuctionsRelated
{
    public class BidDetailEntityConfiguration : IEntityTypeConfiguration<BidDetail>
    {
        public void Configure(EntityTypeBuilder<BidDetail> builder)
        {
            builder.ToTable("BidDetail", "AuctionRel");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.BidId).IsRequired();
            builder.Property(x => x.AuctionRequirementId).IsRequired();
            builder.Property(x => x.Rate).HasPrecision(18, 4).IsRequired();
            builder.Property(x => x.BaseAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.NetAmount).HasPrecision(18, 2).IsRequired();

            builder.HasOne(x => x.AuctionRequirement)
                .WithMany()
                .HasForeignKey(x => x.AuctionRequirementId)
                .HasConstraintName("FK_BidDetail_AuctionRequirementId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Taxes)
                .WithOne(x => x.BidDetail)
                .HasForeignKey(x => x.BidDetailId)
                .HasConstraintName("FK_BidTaxDetail_BidDetailId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
