using System.ComponentModel.DataAnnotations;

namespace gaiacabinet_api.Contracts;

public sealed class UserDto
{
    public int UserId { get; init; }

    [Required]
    public string FirstName { get; init; } = null!;

    [Required]
    public string LastName { get; init; } = null!;

    [Required, EmailAddress]
    public string Email { get; init; } = null!;

    public string? Phone { get; init; }

    public RoleDto Role { get; init; } = new();
}