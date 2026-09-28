using Core.Abstraction.Services;
using Core.Entities.GlobalData;
using Core.Entities.Master;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Master
{
    public class ItemEntityConfiguration(IJwtHelperService jwtHelperService) : IEntityTypeConfiguration<Item>
    {
        private readonly IJwtHelperService _jwtHelper = jwtHelperService;

        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Item", "Master");
            MasterBaseEntityConfiguration.Configure(builder);
            builder.HasQueryFilter(x => x.TenantId == _jwtHelper.GetTenantId());

            builder.Property(x => x.CategoryId).IsRequired();
            builder.Property(x => x.ItemDescription).IsRequired(false);
            builder.Property(x => x.DocAttachmentId).IsRequired(false);

            builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .HasConstraintName("FK_Item_CategoryId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ApplicableUnits)
                .WithOne()
                .HasForeignKey(x => x.ItemId)
                .HasConstraintName("FK_ItemUnitMapping_ItemId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
