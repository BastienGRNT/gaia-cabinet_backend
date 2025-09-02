namespace GaiaSolution.Domain.Entities;

public class UserCredential
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
    public DateTimeOffset PasswordUpdatedAt { get; set; }

    public int FailedLoginCount { get; set; } = 0;
    public DateTimeOffset? LockoutUntil { get; set; }
}