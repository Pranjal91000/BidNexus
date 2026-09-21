using Core.Entities.GlobalData;
using Core.Entities.Master;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Master
{
    public class TaxMasterEntityConfiguration : IEntityTypeConfiguration<TaxMaster>
    {
        public void Configure(EntityTypeBuilder<TaxMaster> builder)
        {
            builder.ToTable("TaxMaster", "Master");
            MasterBaseEntityConfiguration.Configure(builder);

            builder.Property(x => x.TaxNatureId).IsRequired();
            builder.Property(x => x.ChargeTypeId).IsRequired();
            builder.Property(x => x.TaxValue).HasPrecision(18, 4).IsRequired();

            builder.HasOne(x => x.TaxNature)
                .WithMany()
                .HasForeignKey(x => x.TaxNatureId)
                .HasConstraintName("FK_TaxMaster_TaxNatureId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ChargeType)
                .WithMany()
                .HasForeignKey(x => x.ChargeTypeId)
                .HasConstraintName("FK_TaxMaster_ChargeTypeId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Status)
                .WithMany()
                .HasForeignKey(x => x.StatusId)
                .HasConstraintName("FK_TaxMaster_StatusId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
