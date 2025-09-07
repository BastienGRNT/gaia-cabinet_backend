using GaiaSolution.Domain.Base;

namespace GaiaSolution.Domain.Entities;

public class UserSession : BaseAuditableEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string RefreshTokenHash { get; set; } = null!;
    
    public Guid DeviceId { get; set; } 
    
    public DateTimeOffset ExpiresAt { get; set; }
    
    public DateTimeOffset? RevokedAt { get; set; }
    public int? RevokedByUserId { get; set; }
    public User? RevokedByUser { get; set; }
    
    public string? DeviceName { get; set; }
    public string? LastUserAgent { get; set; }
    public string? LastIpAddress { get; set; }
    public DateTimeOffset? LastSeenAt { get; set; }
}