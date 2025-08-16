namespace gaiacabinet_api.Models;

// Tables des Utilisateurs en "liste blanche" (ex : ménage ou medecin autorisé à s’inscrire)
public class PendingUser
{
    public int PendingUserId { get; set; }

    public string Mail { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int InvitedByUserId { get; set; }
    public User InvitedByUser { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ConsumedAt { get; set; }
    
    public string VerificationCodeHash { get; set; } = null!;
    
    public DateTimeOffset? VerificationCodeCreation { get; set; }
    public DateTimeOffset? VerificationCodeExpiration { get; set; }

    public int Attempts { get; set; }

    public bool IsActive { get; set; }
    
    public DateTimeOffset ExpiresAt { get; set; }
}