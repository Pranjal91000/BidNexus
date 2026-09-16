using Core.Entities.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Shared
{
    public static class BaseEntityConfiguration
    {
        public static void Configure<T>(EntityTypeBuilder<T> builder) where T: BaseEntity 
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.TenantId).IsRequired();
        }

    }
}
