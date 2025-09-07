namespace GaiaSolution.Domain.Entities;

public class EmailVerification
{
    public int EmailVerificationId { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string Purpose { get; set; }
    
    public string EmailCodeHash { get; set; } = null!;
    
    public DateTimeOffset ExpiresAt { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ConsumedAt { get; set; }
    
    public int Attempts { get; set; }
    public DateTimeOffset? UnlockAt { get; set; }
}