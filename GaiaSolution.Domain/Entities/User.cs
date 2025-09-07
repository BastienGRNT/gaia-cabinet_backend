using GaiaSolution.Domain.ValueObjects;

namespace GaiaSolution.Domain.Entities;

public enum StatusEnum { Pending, Active, Suspended, Banned, Deleted }

public class User
{
    public int UserId { get; set; }
    
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public StatusEnum Status { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    //TODO : Ne pas oublier le value converter
    public EmailNormalized Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? LastLogin { get; set; }
    
    public DoctorProfile? DoctorProfile { get; set; }
    public UserCredential? Credential { get; set; } 
    public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    public ICollection<UserLoginHistory> LoginHistory { get; set; } = new List<UserLoginHistory>();
}