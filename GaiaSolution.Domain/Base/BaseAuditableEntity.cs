namespace GaiaSolution.Domain.Base;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    
    internal void SetCreated(DateTimeOffset ts) => CreatedAt = ts;
    internal void SetUpdated(DateTimeOffset ts) => UpdatedAt = ts;
}