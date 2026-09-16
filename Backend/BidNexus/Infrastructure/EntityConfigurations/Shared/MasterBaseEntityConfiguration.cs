using Core.Entities.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Shared
{
    public static class MasterBaseEntityConfiguration
    {
        public static void Configure<T>(EntityTypeBuilder<T> builder) where T : MasterBaseEntity
        {
            BaseEntityConfiguration.Configure(builder);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Code).IsRequired();
            builder.Property(x => x.CreatedDateTime).ValueGeneratedOnAdd();
            builder.Property(x => x.LastModifiedDateTime).ValueGeneratedOnAddOrUpdate();
            builder.Property(x => x.StatusId).IsRequired();
            builder.Property(x => x.StatusRemarks).IsRequired(false);
        }
    }
}
