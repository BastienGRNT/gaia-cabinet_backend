using System.ComponentModel.DataAnnotations;

namespace gaiacabinet_api.Contracts;


// ---- LOOKUP-DTOs ----
// Format attendu par le end point /auth/lookup
public sealed class LookupRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;
}
// Statut possible renvoyer par /auth/lookup
public static class LookupStatus
{
    public const string ExistingUser = "existing_user";
    public const string PendingUser  = "pending_user";
    public const string Unknown      = "unknown";
}
// Format de réponse envoyer par l'api /auth/lookup
public sealed class LookupResponse
{
    public string Status { get; init; } = LookupStatus.Unknown;
    public RoleDto? Role { get; init; }
}


// ---- LOGIN-DTOs ----
// Format attendu par le end point /auth/login
public sealed class LoginRequest
{
    [Required, EmailAddress] public string Email { get; init; } = string.Empty;
    [Required] public string Password { get; init; } = string.Empty;
}
// Format de réponse envoyer par l'api /auth/login
public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = string.Empty;
}

