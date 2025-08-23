namespace gaiacabinet_api.Models;

public sealed class RefreshSession
{
    public int SessionId { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string TokenHash { get; set; } = null!;
    public string SessionKeyHash { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    
    public int? RevokedByUserId { get; set; }
    public User? RevokedByUser { get; set; }
    
    public DateTimeOffset? LastSeenAt { get; set; }

    public string? LastIp { get; set; }
    public string? LastUserAgent { get; set; }
}