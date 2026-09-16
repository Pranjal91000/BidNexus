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
            builder.Property(x => x.CreatedDateTime).ValueGeneratedOnAdd();
            builder.Property(x => x.LastModifiedDateTime).ValueGeneratedOnAddOrUpdate();
            builder.Property(x => x.DocDate).IsRequired();
            //builder.HasQueryFilter(x => x.TenantId == )
        }      
    }
}
