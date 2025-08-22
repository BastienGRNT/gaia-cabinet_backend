using System.Security.Cryptography;
using gaiacabinet_api.Contracts;
using gaiacabinet_api.Database;
using gaiacabinet_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace gaiacabinet_api.Services;

// Horloge injectable (meilleure testabilité que DateTimeOffset.UtcNow)
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

public sealed record LookupResult
{
    public string Status;
    public Role? Role;

    public LookupResult(string status, Role? role)
    {
        Status = status;
        Role = role;
    }
}

public sealed record LoginResult
{
    public string AccessToken;
    public string RefreshToken;

    public LoginResult(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}

public sealed record RefreshResult
{
    public string AccessToken;
    public string RefreshToken;

    public RefreshResult(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}


public interface IAuthServices
{
    Task<LookupResult> LookupAsync(string email, CancellationToken ct);
    Task<LoginResult> LoginAsync(string email, string password, string ip, string userAcces, CancellationToken ct);
    Task<RefreshResult> RefreshAsync(string refreshToken, string ip, string userAgent, CancellationToken ct);
}

public sealed class AuthServices : IAuthServices
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly AuthJwtOptions _jwt;
    private readonly ITokenService _jwtService;

    public AuthServices(AppDbContext db, IClock clock, IOptions<AuthJwtOptions> jwtOptions, ITokenService jwtService)
    {
        _db = db;
        _clock = clock;
        _jwt = jwtOptions.Value;
        _jwtService = jwtService;
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
    
    public static string GenerateVerificationCode()
    {
        var number = RandomNumberGenerator.GetInt32(0, 1_000_000); 
        return number.ToString("D6");
    }

    public async Task<LookupResult> LookupAsync(string email, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email))
            return new LookupResult(LookupStatus.Unknown, null);

        var normalized = NormalizeEmail(email);
        
        var userExists = await _db.Users
            .AsNoTracking()
            .AnyAsync(u => EF.Functions.ILike(u.Mail, normalized), ct);

        if (userExists)
            return new LookupResult(LookupStatus.ExistingUser, null);

        var pending = await _db.PendingUsers
            .AsNoTracking()
            .Include(p => p.Role)
            .FirstOrDefaultAsync(
                p => EF.Functions.ILike(p.Mail, normalized)
                     && p.IsActive
                     && p.ExpiresAt > _clock.UtcNow
                     && p.ConsumedAt == null,
                ct);

        if (pending is not null)
        {
            var code = GenerateVerificationCode();
            var hash = BCrypt.Net.BCrypt.HashPassword(code);

            await _db.PendingUsers
                .Where(p => EF.Functions.ILike(p.Mail, normalized) && p.IsActive && p.ExpiresAt > _clock.UtcNow)
                .ExecuteUpdateAsync(set => set
                        .SetProperty(p => p.VerificationCodeHash, hash)
                        .SetProperty(p => p.VerificationCodeCreation, _clock.UtcNow)
                        .SetProperty(p => p.VerificationCodeExpiration, _clock.UtcNow.AddMinutes(5)), 
                    ct);

            // TODO: envoyer `code` si updated > 0
            Console.WriteLine("Votre code de vérification est : " + code);
            return new LookupResult(LookupStatus.PendingUser, pending.Role);
        }

        return new LookupResult(LookupStatus.Unknown, null);
    }

    public async Task<LoginResult> LoginAsync(string email, string password, string ip, string userAgent, CancellationToken ct)
    {
        var normalized = NormalizeEmail(email);
        
        var user = await _db.Users
            .Include(p => p.Role)
            .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Mail, normalized), ct);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("invalid_credentials");
        
        if(!user.Authorized)
            throw new UnauthorizedAccessException("not_authorized");
        
        user.LastLogin = _clock.UtcNow;
        await _db.SaveChangesAsync(ct);

        var accessToken = _jwtService.GenerateAccessToken(user);

        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshHash = _jwtService.HashToken(refreshToken);
        var refreshExpiration = _clock.UtcNow.AddDays(_jwt.RefreshTokenDays);

        _db.RefreshSessions.Add(new RefreshSession
        {
            UserId = user.UserId,
            TokenHash = refreshHash,
            ExpiresAt = refreshExpiration,
            IpAddress = ip,
            UserAgent = userAgent
        });
        await _db.SaveChangesAsync(ct);
        
        return new LoginResult(accessToken, refreshToken);
    }

    public async Task<RefreshResult> RefreshAsync(string refreshToken, string ip, string userAgent, CancellationToken ct)
    {
        var pair = await _jwtService.VerifyAndRotateAsync(refreshToken, ip, userAgent, ct);
        return new RefreshResult(pair.AccessToken, pair.RefreshToken);
    }
}
