using Core.Entities.Auction;
using Core.Entities.GlobalData;
using Core.Entities.TenantRelated;
using Core.Entities.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Utilities
{
    public class RatingEntityConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.ToTable("Rating", "Utilities");

            builder.HasKey(x => new { x.RatingForId, x.AuctionId, x.AgainstTenant, x.SubmittedByTenant });

            builder.Property(x => x.RatingForId).IsRequired();
            builder.Property(x => x.AuctionId).IsRequired();
            builder.Property(x => x.AgainstTenant).IsRequired();
            builder.Property(x => x.SubmittedByTenant).IsRequired();
            builder.Property(x => x.Remark).IsRequired(false);

            builder.HasOne<Auction>()
                .WithMany()
                .HasForeignKey(x => x.AuctionId)
                .HasConstraintName("FK_Rating_AuctionId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(x => x.AgainstTenant)
                .HasConstraintName("FK_Rating_AgainstTenant")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(x => x.SubmittedByTenant)
                .HasConstraintName("FK_Rating_SubmittedByTenant")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
