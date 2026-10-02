using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities.Shared;

namespace Infrastructure.EntityConfigurations.Shared
{
    public static class TransactionBasedEnitityConfiguration
    {
        public static void Configure<T>(EntityTypeBuilder<T> builder) where T: TransactionBaseEntity
        {
            BaseEntityConfiguration.Configure(builder);
            builder.Property(x => x.CreatedDateTime).IsRequired();
            builder.Property(x => x.LastModifiedDateTime).IsRequired();
            builder.Property(x => x.DocDate).IsRequired();
            //builder.HasQueryFilter(x => x.TenantId == )
        }      
    }
}
