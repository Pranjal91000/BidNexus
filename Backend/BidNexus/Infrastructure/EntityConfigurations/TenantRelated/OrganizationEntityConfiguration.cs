using Core.Entities.TenantRelated;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.TenantRelated
{
    public class OrganizationEntityConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.ToTable("Organization", "TenantRel");
            BaseEntityConfiguration.Configure(builder);

            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.OfficialAddress).IsRequired(false).HasMaxLength(500);
            builder.Property(x => x.ForegroundImageId).IsRequired(false);
            builder.Property(x => x.About).IsRequired(false).HasMaxLength(5000);

            builder.HasOne(x => x.Tenant)
                .WithMany()
                .HasConstraintName("FK_Organization_TenantId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
