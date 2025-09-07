using GaiaSolution.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaiaSolution.Infrastructure.Configurations;

public class AuditEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : AuditableEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();

        builder.Property(e => e.UpdatedAt);
    }
}
