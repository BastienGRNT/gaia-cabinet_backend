// File: Services/TokenService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using gaiacabinet_api.Database;
using gaiacabinet_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace gaiacabinet_api.Services;

public sealed record TokenPair(
    string AccessToken,
    string RefreshToken
);

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashToken(string token);
    Task<TokenPair> VerifyAndRotateAsync(
        string refreshToken,
        string? ip,
        string? userAgent,
        CancellationToken ct);
}

public sealed class TokenService : ITokenService
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly AuthJwtOptions _jwt;

    public TokenService(AppDbContext db, IClock clock, IOptions<AuthJwtOptions> jwtOptions)
    {
        _db = db;
        _clock = clock;
        _jwt = jwtOptions.Value;
    }

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = _clock.UtcNow;
        var expires = now.AddMinutes(_jwt.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(ClaimTypes.Role, user.Role?.Label ?? "user"),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var jwt = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: creds);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (token);
    }

    public string GenerateRefreshToken()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    public string HashToken(string token)
    {
        var data = Encoding.UTF8.GetBytes(token);
        return Convert.ToHexString(SHA256.HashData(data));
    }

    public async Task<TokenPair> VerifyAndRotateAsync(string refreshToken, string? ip, string? userAgent, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("invalid_refresh_token");

        var now = _clock.UtcNow;
        var incomingHash = HashToken(refreshToken);

        var session = await _db.RefreshSessions
            .Include(s => s.User).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(s => s.TokenHash == incomingHash, ct);

        if (session is null)
            throw new UnauthorizedAccessException("invalid_refresh_token");

        if (session.RevokedAt is not null)
            throw new UnauthorizedAccessException("revoked_refresh_token");

        if (session.ExpiresAt <= now)
            throw new UnauthorizedAccessException("expired_refresh_token");

        var user = session.User;
        if (user is null || !user.Authorized)
            throw new UnauthorizedAccessException("not_authorized");

        var newAccess = GenerateAccessToken(user);

        // 4) Rotation du refresh : créer un nouveau RT, enregistrer, révoquer l’ancien
        var newRt = GenerateRefreshToken();
        var newRtHash = HashToken(newRt);
        var newRtExp = now.AddDays(_jwt.RefreshTokenDays);

        // Marquer l'ancien comme révoqué + chainage
        session.RevokedAt = now;

        // Créer la nouvelle session
        _db.RefreshSessions.Add(new RefreshSession
        {
            UserId = user.UserId,
            TokenHash = newRtHash,
            ExpiresAt = newRtExp,
            IpAddress = ip,
            UserAgent = userAgent
        });

        await _db.SaveChangesAsync(ct);

        return new TokenPair(newAccess, newRt);
    }

    private static string Base64UrlEncode(ReadOnlySpan<byte> bytes)
        => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
