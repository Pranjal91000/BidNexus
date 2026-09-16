using Core.Entities.GlobalData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.GlobalData
{
    public class RatingParameterEntityConfiguration : IEntityTypeConfiguration<RatingParameter>
    {
        public void Configure(EntityTypeBuilder<RatingParameter> builder)
        {
            builder.ToTable("RatingParameter", "GlobalData");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.ParameterName).IsRequired();
            builder.Property(x => x.RatingFor).IsRequired();
        }
    }
}
