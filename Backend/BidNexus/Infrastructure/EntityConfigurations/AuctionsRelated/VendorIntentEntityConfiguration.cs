using Core.Abstraction.Services;
using Core.Entities.Auction;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.AuctionsRelated
{
    public class VendorIntentEntityConfiguration(IJwtHelperService jwtHelperService): IEntityTypeConfiguration<VendorIntent>
    {
        private readonly IJwtHelperService _jwtHelper = jwtHelperService;

        public void Configure(EntityTypeBuilder<VendorIntent> builder)
        {
            builder.ToTable("VendorIntent", "AuctionRel");
            BaseEntityConfiguration.Configure(builder);
            builder.HasQueryFilter(x => x.TenantId == _jwtHelper.GetTenantId());
            builder.Property(x => x.AuctionId).IsRequired();
            builder.Property(x => x.VendorId).IsRequired();
            builder.Property(x => x.IsInterested).IsRequired();
            builder.Property(x => x.IsQualified).IsRequired();

            builder.HasOne(x => x.Auction)
                .WithMany(x => x.VendorIntent)
                .HasForeignKey(x => x.AuctionId)
                .HasConstraintName("Fk_VendorIntent_AuctionId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .HasConstraintName("Fk_VendorIntent_VendorId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
