using GaiaSolution.Domain.Base;

namespace GaiaSolution.Domain.Entities;

public class UserLoginHistory : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTimeOffset LoginAt { get; set; }
    public bool Succeeded { get; set; }
    
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}