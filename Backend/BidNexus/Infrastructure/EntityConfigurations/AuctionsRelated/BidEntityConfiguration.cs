using Core.Entities.Auction;
using Core.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.AuctionsRelated
{
    public class BidEntityConfiguration : IEntityTypeConfiguration<Bid>
    {
        public void Configure(EntityTypeBuilder<Bid> builder)
        {
            builder.ToTable("Bid", "AuctionRel");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.IsCurrent).IsRequired();
            builder.Property(x => x.MainBidId).IsRequired();
            builder.Property(x => x.AuctionId).IsRequired();
            builder.Property(x => x.VendorId).IsRequired();
            builder.Property(x => x.FinalPrice).IsRequired();
            builder.Property(x => x.CreatedAt).ValueGeneratedOnAdd();
            builder.Property(x => x.BidRevisionNo).IsRequired();

            builder.HasOne(x => x.MainBid)
                .WithMany()
                .HasForeignKey(x => x.MainBidId)
                .HasConstraintName("FK_Bid_MainBidId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Auction)
                .WithMany(x => x.Bids)
                .HasForeignKey(x => x.AuctionId)
                .HasConstraintName("FK_Bid_AuctionId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .HasConstraintName("FK_Bid_VendorId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.BidDetails)
                .WithOne()
                .HasForeignKey(x => x.BidId)
                .HasConstraintName("FK_BidDetail_BidId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
