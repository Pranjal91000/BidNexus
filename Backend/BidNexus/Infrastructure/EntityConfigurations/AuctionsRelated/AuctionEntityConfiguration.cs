using Core.Abstraction.Services;
using Core.Entities.Auction;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.AuctionsRelated
{
    public class AuctionEntityConfiguration(IJwtHelperService jwtHelperService): IEntityTypeConfiguration<Auction>
    {
        private readonly IJwtHelperService _jwtHelper = jwtHelperService;
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            builder.ToTable("Auction", "AuctionRel");
            TransactionBasedEnitityConfiguration.Configure(builder);
            builder.Property(x => x.IsBidPriceHidden).IsRequired();
            builder.Property(x => x.IsForwardAuction).IsRequired();
            builder.Property(x => x.OpenToAll).IsRequired();
            builder.Property(x => x.DocAttachmentId).IsRequired(false);
            builder.Property(x => x.AuctionEndTime).IsRequired();
            builder.Property(x => x.AuctionStartTime).IsRequired();
            builder.Property(x => x.OrganizationId).IsRequired();
            builder.Property(x => x.StatusId).IsRequired();

            builder.HasOne(x => x.Organization)
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .HasConstraintName("FK_Auction_OrganizationId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Status)
                .WithMany()
                .HasForeignKey(x => x.StatusId)
                .HasConstraintName("FK_Auction_StatusId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => x.TenantId == _jwtHelper.GetTenantId());
        }
    }
}
