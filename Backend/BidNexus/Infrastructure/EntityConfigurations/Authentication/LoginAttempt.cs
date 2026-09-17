using Core.Entities.Authentication;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Authentication
{
    public class LoginAttemptEntityConfiguration: IEntityTypeConfiguration<LoginAttempt>
    {
        public void Configure(EntityTypeBuilder<LoginAttempt> builder)
        {
            builder.ToTable("LoginAttempt", "Auth");
            BaseEntityConfiguration.Configure(builder);
            builder.Property(x => x.FailedLoginAttemptCount).IsRequired();
            builder.Property(x => x.LastAttemptedOn).IsRequired();
            builder.Property(x => x.StatusId).IsRequired();
            builder.HasOne(x => x.Status)
                .WithOne()
                .HasForeignKey<LoginAttempt>(x => x.StatusId)
                .HasConstraintName("Fk_LoginAttempt_StatusId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
