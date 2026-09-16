using Core.Entities.Auction;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.AuctionsRelated
{
    public class AuctionIRequirementEntityConfiguration: IEntityTypeConfiguration<AuctionRequirement>
    {
        public void Configure(EntityTypeBuilder<AuctionRequirement> builder)
        {
            builder.ToTable("AuctionRequirement", "AuctionRel");

            BaseEntityConfiguration.Configure(builder);

            builder.Property(x => x.AuctionId).IsRequired();

            builder.Property(x => x.ItemId).IsRequired();

            builder.Property(x => x.TechnicalSpecification).IsRequired(false);

            builder.Property(x => x.Quantity).IsRequired();

            builder.Property(x => x.Quantity).HasPrecision(2);

            builder.Property(x => x.UnitId).IsRequired();

            builder.HasOne(x => x.Auction)
                .WithMany()
                .HasForeignKey(x => x.AuctionId)
                .HasConstraintName("FK_AuctionRequirement_AuctioId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .HasConstraintName("FK_AuctionRequirement_ItemId")
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
