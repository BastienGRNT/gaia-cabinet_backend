using GaiaSolution.Application.Base.Interfaces;
using GaiaSolution.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GaiaSolution.Infrastructure.Database.Interceptors;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IClock _clock;
    public AuditSaveChangesInterceptor(IClock clock) => _clock = clock;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        StampAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        StampAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void StampAudit(DbContext? ctx)
    {
        if (ctx is null) return;
        var now = _clock.UtcNow;
        
        foreach (var entry in ctx.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(BaseAuditableEntity.CreatedAt)).CurrentValue = now;
                entry.Property(nameof(BaseAuditableEntity.UpdatedAt)).CurrentValue = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(BaseAuditableEntity.CreatedAt)).IsModified = false;
                entry.Property(nameof(BaseAuditableEntity.UpdatedAt)).CurrentValue = now;
            }
        }
    }
}