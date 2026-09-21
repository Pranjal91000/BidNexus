using Core.Entities.Tenant;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.TenantRelated
{
    public class VendorEntityConfiguration : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {
            builder.ToTable("Vendor", "Tenant");
            BaseEntityConfiguration.Configure(builder);

            builder.Property(x => x.ForegroundImageId).IsRequired(false);
            builder.Property(x => x.About).IsRequired(false);
        }
    }
}
