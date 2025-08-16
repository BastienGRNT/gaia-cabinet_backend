namespace gaiacabinet_api.Models;

// Table principale des utilisateurs
public class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Mail { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? OrdreRegistrationNumber { get; set; }

    public short? DaysAdvance { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? LastLogin { get; set; }
    
    public Boolean Authorized { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}