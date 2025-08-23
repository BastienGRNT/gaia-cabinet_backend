using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using gaiacabinet_api.Common;
using gaiacabinet_api.Database;
using gaiacabinet_api.Interfaces;
using gaiacabinet_api.Models;
using gaiacabinet_api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace gaiacabinet_api.Services;

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

    // Methode de géneration d'un AccessToken
    public string GenerateAccessToken(User user)
    {
        //Récuperer la clé secrète dans la class JwtOptions
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

    // Méthode de genreation d'un RefreshToken
    public string GenerateRefreshToken()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return TokenUtils.Base64UrlEncode(bytes);
    }

    // Méthode de Hash utiliser pour ajouter le RefreshTokenHash en BDD
    public string HashToken(string token)
    {
        var data = Encoding.UTF8.GetBytes(token);
        return Convert.ToHexString(SHA256.HashData(data));
    }

    // Méthode pour vérifier un RefreshToken et en créer un nouveau si il est bon
    public async Task<VerifyAndRotateResult> VerifyAndRotateAsync(string refreshToken, string sessionKey, string? ip, string? userAgent, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("invalid_refresh_token");
        
        if (string.IsNullOrWhiteSpace(sessionKey))
            throw new UnauthorizedAccessException("invalid_session_key");

        var now = _clock.UtcNow;
        var incomingHash = TokenUtils.HashToken(refreshToken);
        var incomingSessionKey = TokenUtils.HashToken(sessionKey);

        var session = await _db.RefreshSessions
            .Include(s => s.User).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(s => s.TokenHash == incomingHash
                && s.SessionKeyHash == incomingSessionKey
                , ct);

        if (session is null)
            throw new UnauthorizedAccessException("invalid_refresh_token");

        if (session.RevokedAt is not null)
            throw new UnauthorizedAccessException("revoked_refresh_token");

        if (session.ExpiresAt <= now)
        {
            await _db.RefreshSessions
                .Where(s => s.SessionId == session.SessionId && s.RevokedAt == null)
                .ExecuteUpdateAsync(set => set
                    .SetProperty(x => x.RevokedAt, now), ct);
        
            throw new UnauthorizedAccessException("expired_refresh_token");
        }
        
        var user = session.User;
        if (user is null || !user.Authorized)
        {
            await _db.RefreshSessions
                .Where(s => s.SessionId == session.SessionId && s.RevokedAt == null)
                .ExecuteUpdateAsync(set => set
                    .SetProperty(x => x.RevokedAt, now), ct);
            
            throw new UnauthorizedAccessException("not_authorized");
        }

        var newAccess = GenerateAccessToken(user);

        // 4) Rotation du refresh : créer un nouveau RT, enregistrer, révoquer l’ancien
        var newRt = GenerateRefreshToken();
        var newRtHash = TokenUtils.HashToken(newRt);
        var newRtExp = now.AddDays(_jwt.RefreshTokenDays);
        
        
        var updated = await _db.RefreshSessions
            .Where(s => s.SessionKeyHash == incomingSessionKey
                && s.TokenHash == incomingHash
                && s.RevokedAt == null
                && s.ExpiresAt > now)
            .ExecuteUpdateAsync(set => set
                .SetProperty(x => x.TokenHash, newRtHash)
                .SetProperty(x => x.ExpiresAt, newRtExp)
                .SetProperty(x => x.UpdatedAt, now)
                .SetProperty(x => x.LastSeenAt, now)
                .SetProperty(x => x.LastIp, x => string.IsNullOrWhiteSpace(ip) ? x.LastIp : ip)
                .SetProperty(x => x.LastUserAgent, x => string.IsNullOrWhiteSpace(userAgent) ? x.LastUserAgent : userAgent)
            , ct);
        
        if (updated == 0)
            throw new UnauthorizedAccessException("not_authorized");
        
        return new VerifyAndRotateResult(newAccess, newRt);
    }

    
}
