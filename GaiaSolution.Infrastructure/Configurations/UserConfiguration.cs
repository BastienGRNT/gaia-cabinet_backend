using GaiaSolution.Domain.Entities;
using GaiaSolution.Domain.Enums;
using GaiaSolution.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaiaSolution.Infrastructure.Configurations;

public sealed class UserConfiguration :IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Status)
            .HasConversion(
                u => u.ToString(),
                u => (StatusEnum)Enum.Parse(typeof(StatusEnum), u));
        
        builder.Property(u => u.FirstName).HasMaxLength(50);
        builder.Property(u => u.LastName).HasMaxLength(50);

        builder.Property(u => u.EmailNormalized)
            .HasConversion(
                v => v.Value,
                u => EmailNormalized.From(u))
            .HasMaxLength(350)
            .IsRequired();

        builder.Property(u => u.PhoneNormalized)
            .HasConversion(
                v => v.Value,
                u => PhoneNormalized.From(u))
            .HasMaxLength(12)
            .IsRequired();
        
        builder.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.Id);
        builder.HasIndex(u => u.EmailNormalized).IsUnique();
        builder.HasIndex(u => u.PhoneNormalized).IsUnique();
    }
}