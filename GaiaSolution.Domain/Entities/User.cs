using GaiaSolution.Domain.Base;
using GaiaSolution.Domain.Enums;
using GaiaSolution.Domain.ValueObjects;

namespace GaiaSolution.Domain.Entities;

public class User : BaseEntity
{
    public StatusEnum Status { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public EmailNormalized EmailNormalized { get; set; } = null!;
    public PhoneNormalized PhoneNormalized { get; set; } = null!;
    
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public DoctorProfile? UserDoctorProfile { get; set; }
    public UserCredential? UserCredential { get; set; } 
    public ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    public ICollection<UserSession> UserSessionsRevoked { get; set; } = new List<UserSession>();
    public ICollection<UserLoginHistory> UserLoginHistory { get; set; } = new List<UserLoginHistory>();
    public ICollection<EmailVerification> UserEmailVerification { get; set; } = new List<EmailVerification>();
}