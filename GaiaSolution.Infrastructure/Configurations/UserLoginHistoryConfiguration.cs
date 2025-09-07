using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GaiaSolution.Domain.Entities;

namespace GaiaSolution.Infrastructure.Configurations;

public sealed class UserLoginHistoryConfiguration : IEntityTypeConfiguration<UserLoginHistory>
{
    public void Configure(EntityTypeBuilder<UserLoginHistory> builder)
    {
        builder.HasOne(ul => ul.User)
            .WithMany(u => u.LoginHistory)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(ul => ul.LoginAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();
        
        builder.Property(ul => ul.Succeeded)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(ul => ul.IpAddress)
            .HasMaxLength(256)
            .HasDefaultValue(null);
        
        builder.Property(ul => ul.UserAgent)
            .HasMaxLength(256)
            .HasDefaultValue(null);

        builder.Property(ul => ul.UserId);
    }
}