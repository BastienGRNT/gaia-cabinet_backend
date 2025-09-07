namespace GaiaSolution.Domain.Base;

public abstract class AuditableEntity : BaseEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}