namespace gaiacabinet_api.Models;

// Tables des Utilisateurs en "liste blanche" (ex : ménage ou medecin autorisé à s’inscrire)
public class PendingUser
{
    // --- Identité ---
    public int PendingUserId { get; set; }
    public string Email { get; set; } = null!;

    // --- Références ---
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public int InvitedByUserId { get; set; }
    public User InvitedByUser { get; set; } = null!;

    // --- Durée de vie globale ---
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? ConsumedAt { get; set; }
    
    // --- Revocation du pending ---
    public bool IsActive { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    

    // --- Vérification par code ---
    public string? VerificationCodeHash { get; set; }
    public DateTimeOffset? VerificationCodeCreation { get; set; }
    public DateTimeOffset? VerificationCodeExpiration { get; set; }

    // --- Sécurité / anti-abus ---
    public int Attempts { get; set; }
    public DateTimeOffset? UnlockAt { get; set; }

    // --- Validation finale ---
    public string? ValidateTokenHash { get; set; }
    public DateTimeOffset? ValidateTokenCreation { get; set; }
    public DateTimeOffset? ValidateTokenExpiration { get; set; }
}