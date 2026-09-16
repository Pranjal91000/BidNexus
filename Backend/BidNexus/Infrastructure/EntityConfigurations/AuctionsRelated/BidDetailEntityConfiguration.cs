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
            builder.Property(x => x.Price).IsRequired();

            builder.HasOne<AuctionRequirement>()
                .WithMany()
                .HasForeignKey(x => x.AuctionRequirementId)
                .HasConstraintName("FK_BidDetail_AuctionRequirementId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
