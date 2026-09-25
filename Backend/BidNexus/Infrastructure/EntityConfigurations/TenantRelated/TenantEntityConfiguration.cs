using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities.TenantRelated;

namespace Infrastructure.EntityConfigurations.TenantRelated
{
    public class TenantEntityConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenant", "Tenant");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Name).HasMaxLength(500).IsRequired();
            builder.Property(x => x.ContactNumber).IsRequired();
            builder.Property(x => x.EmailAddress).IsRequired();
            builder.Property(x => x.UserName).IsRequired();
            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.IsVendor).IsRequired();

            builder.HasIndex(x => x.UserName).IsUnique();
            builder.HasIndex(x => x.EmailAddress).IsUnique();
            builder.Property(x => x.IsBlocked).IsRequired();
        }
    }
}
