using Core.Entities.Auction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.AuctionsRelated
{
    public class BidTaxDetailEntityConfiguration : IEntityTypeConfiguration<BidTaxDetail>
    {
        public void Configure(EntityTypeBuilder<BidTaxDetail> builder)
        {
            builder.ToTable("BidTaxDetail", "AuctionRel");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.BidDetailId).IsRequired();
            builder.Property(x => x.TaxId).IsRequired(false);
            builder.Property(x => x.TaxName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.TaxNatureId).IsRequired();
            builder.Property(x => x.ChargeTypeId).IsRequired();
            builder.Property(x => x.TaxValue).HasPrecision(18, 4).IsRequired();
            builder.Property(x => x.TaxAmount).HasPrecision(18, 2).IsRequired();

            builder.HasOne(x => x.BidDetail)
                .WithMany(x => x.Taxes)
                .HasForeignKey(x => x.BidDetailId)
                .HasConstraintName("FK_BidTaxDetail_BidDetailId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Tax)
                .WithMany()
                .HasForeignKey(x => x.TaxId)
                .HasConstraintName("FK_BidTaxDetail_TaxId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TaxNature)
                .WithMany()
                .HasForeignKey(x => x.TaxNatureId)
                .HasConstraintName("FK_BidTaxDetail_TaxNatureId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ChargeType)
                .WithMany()
                .HasForeignKey(x => x.ChargeTypeId)
                .HasConstraintName("FK_BidTaxDetail_ChargeTypeId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
