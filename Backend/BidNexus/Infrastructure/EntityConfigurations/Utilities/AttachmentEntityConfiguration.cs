using Core.Entities.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Utilities
{
    public class AttachmentEntityConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.ToTable("Attachment", "Utilities");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TenantId).IsRequired();
            builder.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(260);
            builder.Property(x => x.StoredFileName).IsRequired().HasMaxLength(260);
            builder.Property(x => x.ContentType).IsRequired().HasMaxLength(100);
            builder.Property(x => x.FileSize).IsRequired();
            builder.Property(x => x.RelativePath).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.CreatedDateTime).IsRequired();

            builder.HasIndex(x => x.TenantId);
        }
    }
}
