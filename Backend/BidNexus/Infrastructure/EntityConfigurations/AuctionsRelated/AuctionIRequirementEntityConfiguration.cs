using Core.Abstraction.Services;
using Core.Entities.Auction;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.AuctionsRelated
{
    public class AuctionIRequirementEntityConfiguration(IJwtHelperService jwtHelperService): IEntityTypeConfiguration<AuctionRequirement>
    {
        private readonly IJwtHelperService _jwtHelper = jwtHelperService;

        public void Configure(EntityTypeBuilder<AuctionRequirement> builder)
        {
            builder.ToTable("AuctionRequirement", "AuctionRel");

            BaseEntityConfiguration.Configure(builder);

            builder.HasQueryFilter(x => x.TenantId == _jwtHelper.GetTenantId());

            builder.Property(x => x.AuctionId).IsRequired();

            builder.Property(x => x.ItemId).IsRequired();

            builder.Property(x => x.TechnicalSpecification).IsRequired(false);

            builder.Property(x => x.Quantity).HasPrecision(18, 3).IsRequired();

            builder.Property(x => x.UnitId).IsRequired();

            builder.Property(x => x.DocumentAttachmentId).IsRequired(false);

            builder.HasOne(x => x.Unit)
                .WithMany()
                .HasForeignKey(x => x.UnitId)
                .HasConstraintName("FK_AuctionRequirement_UnitId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .HasConstraintName("FK_AuctionRequirement_ItemId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Auction)
                .WithMany(x => x.AuctionRequirements)
                .HasForeignKey(x => x.AuctionId)
                .HasConstraintName("FK_AuctionRequirement_AuctioId")
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
