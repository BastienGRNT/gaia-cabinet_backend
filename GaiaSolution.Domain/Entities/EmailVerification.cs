using GaiaSolution.Domain.Base;
using GaiaSolution.Domain.Enums;

namespace GaiaSolution.Domain.Entities;

public class EmailVerification : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public PurposeEnum Purpose { get; set; }
    
    public string OtpHash { get; set; } = null!;
    
    public DateTimeOffset ExpiresAt { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ConsumedAt { get; set; }
    
    public int Attempts { get; set; }
    public DateTimeOffset? UnlockAt { get; set; }
}