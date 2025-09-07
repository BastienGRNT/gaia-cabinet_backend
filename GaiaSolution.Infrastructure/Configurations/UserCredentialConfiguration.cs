using GaiaSolution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaiaSolution.Infrastructure.Configurations;

public sealed class UserCredentialConfiguration : IEntityTypeConfiguration<UserCredential>
{
    public void Configure(EntityTypeBuilder<UserCredential> builder)
    {
        builder.HasOne(u => u.User)
            .WithOne(uc => uc.Credential)
            .HasForeignKey<UserCredential>(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(uc => uc.PasswordHash)
            .HasMaxLength(72)
            .IsRequired();

        builder.Property(uc => uc.PasswordUpdatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();
        
        builder.Property(uc => uc.FailedLoginCount)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(uc => uc.LockoutUntil);
        
        builder.HasIndex(uc => uc.UserId);
    }
}