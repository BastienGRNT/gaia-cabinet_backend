using gaiacabinet_api.Models;

namespace gaiacabinet_api.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashToken(string token);
    Task<VerifyAndRotateResult> VerifyAndRotateAsync(
        string refreshToken,
        string? ip,
        string? userAgent,
        CancellationToken ct);
}

public sealed record VerifyAndRotateResult(
    string AccessToken,
    string RefreshToken
);
