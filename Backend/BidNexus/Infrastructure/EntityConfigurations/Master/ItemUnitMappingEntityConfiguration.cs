using Core.Entities.Master;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Master
{
    public class ItemUnitMappingEntityConfiguration : IEntityTypeConfiguration<ItemUnitMapping>
    {
        public void Configure(EntityTypeBuilder<ItemUnitMapping> builder)
        {
            builder.ToTable("ItemUnitMapping", "Master");
            BaseEntityConfiguration.Configure(builder);

            builder.Property(x => x.ItemId).IsRequired();
            builder.Property(x => x.UnitId).IsRequired();

            builder.HasOne<Unit>()
                .WithMany()
                .HasForeignKey(x => x.UnitId)
                .HasConstraintName("FK_ItemUnitMapping_UnitId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
