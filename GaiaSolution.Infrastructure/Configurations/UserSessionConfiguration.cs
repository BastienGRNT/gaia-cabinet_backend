using GaiaSolution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaiaSolution.Infrastructure.Configurations;

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.HasOne(u => u.User)
            .WithMany(us => us.UserSessions)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(us => us.RefreshTokenHash).HasMaxLength(256).IsRequired();
        
        builder.Property(us => us.DeviceId).HasMaxLength(128).IsRequired();
        
        builder.Property(us => us.ExpiresAt).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC' + INTERVAL '15 days'").IsRequired();
        
        builder.HasOne(u => u.User)
            .WithMany(us => us.UserSessionsRevoked)
            .HasForeignKey(u => u.RevokedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(u => u.DeviceName).HasMaxLength(256).HasDefaultValue(null);
        builder.Property(u => u.LastUserAgent).HasMaxLength(256).HasDefaultValue(null);
        builder.Property(u => u.LastIpAddress).HasMaxLength(256).HasDefaultValue(null);
        
        builder.Property(u => u.LastSeenAt).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        
    }

}