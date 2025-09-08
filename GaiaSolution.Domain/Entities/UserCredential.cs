using GaiaSolution.Domain.Base;

namespace GaiaSolution.Domain.Entities;

public class UserCredential : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
    public DateTimeOffset PasswordUpdatedAt { get; set; }

    public int FailedLoginCount { get; set; }
    public DateTimeOffset? LockoutUntil { get; set; }
}