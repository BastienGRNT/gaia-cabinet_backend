using GaiaSolution.Domain.Entities;
using GaiaSolution.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaiaSolution.Infrastructure.Persistence.Configurations;

public class DoctorProfileConfiguration : IEntityTypeConfiguration<DoctorProfile>
{
    public void Configure(EntityTypeBuilder<DoctorProfile> builder)
    {
        builder.HasOne(d => d.User)
            .WithOne(u => u.DoctorProfile)
            .HasForeignKey<DoctorProfile>(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(d => d.PostalAddress)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Property(d => d.Rpps)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(d => d.Mss)
            .HasConversion(v => v.Value,
                d => EmailNormalized.From(d))
            .HasMaxLength(350)
            .IsRequired();
        
        builder.Property(d => d.DaysAdvance)
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasIndex(d => d.UserId).IsUnique();
        builder.HasIndex(d => d.Rpps).IsUnique();
        builder.HasIndex(d => d.Mss).IsUnique();
    }
}