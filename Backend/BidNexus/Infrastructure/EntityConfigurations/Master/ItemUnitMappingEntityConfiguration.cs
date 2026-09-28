using Core.Abstraction.Services;
using Core.Entities.Master;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Master
{
    public class ItemUnitMappingEntityConfiguration(IJwtHelperService jwtHelperService) : IEntityTypeConfiguration<ItemUnitMapping>
    {
        private readonly IJwtHelperService _jwtHelper = jwtHelperService;

        public void Configure(EntityTypeBuilder<ItemUnitMapping> builder)
        {
            builder.ToTable("ItemUnitMapping", "Master");
            BaseEntityConfiguration.Configure(builder);
            builder.HasQueryFilter(x => x.TenantId == _jwtHelper.GetTenantId());

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
