using GaiaSolution.Domain.Entities;
using GaiaSolution.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaiaSolution.Infrastructure.Database.Configurations;

public class EmailVerificationConfiguration : IEntityTypeConfiguration<EmailVerification>
{
    public void Configure(EntityTypeBuilder<EmailVerification> builder)
    {
        builder.HasOne(e => e.User)
            .WithMany(u => u.EmailVerification)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(e => e.EmailVerificationPurpose)
            .HasConversion(e => e.ToString(),
                e => (EmailVerificationPurpose)Enum.Parse(typeof(EmailVerificationPurpose), e));
        
        builder.Property(e => e.OtpHash)
            .HasMaxLength(128)
            .IsRequired();
        
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();
        
        builder.Property(e => e.ExpiresAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC' + INTERVAL '5 minute'")
            .IsRequired();

        builder.Property(e => e.ConsumedAt);
        
        builder.Property(e => e.Attempts)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(e => e.UnlockAt);

        builder.HasIndex(e => e.UserId);
    }
}