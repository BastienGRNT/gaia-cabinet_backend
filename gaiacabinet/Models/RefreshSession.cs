namespace gaiacabinet_api.Models;

public sealed class RefreshSession
{
    public int SessionId { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string TokenHash { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}