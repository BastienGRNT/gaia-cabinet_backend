using GaiaSolution.Domain.Base;
using GaiaSolution.Domain.Enums;
using GaiaSolution.Domain.ValueObjects;

namespace GaiaSolution.Domain.Entities;

public class User : BaseEntity
{
    public UserStatus UserStatus { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public EmailNormalized EmailNormalized { get; set; } = null!;
    public PhoneNormalized PhoneNormalized { get; set; } = null!;
    
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public DoctorProfile? DoctorProfile { get; set; }
    public UserCredential? Credential { get; set; } 
    public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    public ICollection<UserSession> SessionsRevoked { get; set; } = new List<UserSession>();
    public ICollection<UserLoginHistory> LoginHistory { get; set; } = new List<UserLoginHistory>();
    public ICollection<EmailVerification> EmailVerification { get; set; } = new List<EmailVerification>();
}