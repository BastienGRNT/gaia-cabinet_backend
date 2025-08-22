using System.ComponentModel.DataAnnotations;

namespace gaiacabinet_api.Contracts;

public static class LookupStatus
{
    public const string ExistingUser = "existing_user";
    public const string PendingUser  = "pending_user";
    public const string Unknown      = "unknown";
}

public sealed class LookupRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;
}

public sealed class LookupResponse
{
    public string Status { get; init; } = LookupStatus.Unknown;
    public RoleDto? Role { get; init; }
}

public sealed class LoginRequest
{
    [Required, EmailAddress] public string Email { get; init; } = string.Empty;
    [Required] public string Password { get; init; } = string.Empty;
}

public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = string.Empty;
}

