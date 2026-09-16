using Core.Entities.GlobalData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.GlobalData
{
    public class RatingForEntityConfiguration : IEntityTypeConfiguration<RatingFor>
    {
        public void Configure(EntityTypeBuilder<RatingFor> builder)
        {
            builder.ToTable("RatingFor", "GlobalData");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.For).IsRequired();
        }
    }
}
