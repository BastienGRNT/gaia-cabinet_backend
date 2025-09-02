using GaiaSolution.Domain.ValueObjects;

namespace GaiaSolution.Domain.Entities;

public enum StatusEnum { Pending, Active, Suspended, Banned, Deleted }
public enum RoleEnum { Admin, Doctor, Cleaning }

public class User
{
    public int UserId { get; set; }
    
    public RoleEnum Role { get; set; }
    public StatusEnum Status { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    //TODO : Ne pas oublier le value converter - et réflechir sur garder ca ou non
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