using Core.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.TenantRelated
{
    public class TenantEntityConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenant", "Tenant");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.ContactNumber).IsRequired();
            builder.Property(x => x.EmailAddress).IsRequired();
            builder.Property(x => x.UserName).IsRequired();
            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.IsVendor).IsRequired();
            builder.Property(x => x.ReferenceId).IsRequired();

            builder.HasIndex(x => x.UserName).IsUnique();
            builder.HasIndex(x => x.EmailAddress).IsUnique();
            builder.Property(x => x.IsBlocked).IsRequired();
        }
    }
}
