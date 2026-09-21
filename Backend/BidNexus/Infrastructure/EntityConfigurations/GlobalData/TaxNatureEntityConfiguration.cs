using Core.Entities.GlobalData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.GlobalData
{
    public class TaxNatureEntityConfiguration : IEntityTypeConfiguration<TaxNature>
    {
        public void Configure(EntityTypeBuilder<TaxNature> builder)
        {
            builder.ToTable("TaxNature", "GlobalData");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();
        }
    }
}
