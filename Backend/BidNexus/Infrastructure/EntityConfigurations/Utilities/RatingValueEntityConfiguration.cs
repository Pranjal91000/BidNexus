using Core.Entities.GlobalData;
using Core.Entities.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Utilities
{
    public class RatingValueEntityConfiguration : IEntityTypeConfiguration<RatingValue>
    {
        public void Configure(EntityTypeBuilder<RatingValue> builder)
        {
            builder.ToTable("RatingValue", "Utilities");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.RatingParameterId).IsRequired();
            builder.Property(x => x.RatingScore).IsRequired();

            builder.HasOne(x => x.RatingParameter)
                .WithMany()
                .HasForeignKey(x => x.RatingParameterId)
                .HasConstraintName("FK_RatingValue_RatingParameterId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
