using Core.Entities.TenantRelated;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.TenantRelated
{
    public class VendorEntityConfiguration : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {
            builder.ToTable("Vendor", "TenantRel");
            BaseEntityConfiguration.Configure(builder);

            builder.Property(x => x.ForegroundImageId).IsRequired(false);
            builder.Property(x => x.About).IsRequired(false);

            builder.HasOne(x => x.Tenant)
                .WithMany()
                .HasConstraintName("FK_Organization_TenantId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
