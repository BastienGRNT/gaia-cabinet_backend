using gaiacabinet_api.Contracts;          // pour LookupStatus
using gaiacabinet_api.Database;
using gaiacabinet_api.Models;
using Microsoft.EntityFrameworkCore;

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


public interface IAuthServices
{
    Task<LookupResult> LookupAsync(string email, CancellationToken ct);
}

public sealed class AuthServices : IAuthServices
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public AuthServices(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

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
                     && p.ExpiresAt > _clock.UtcNow,
                ct);

        if (pending is not null)
            // ENVOYER MAIL VERIFICATION ET AJOUTER LE NUMERO DE VERIFICATION EN BDD
            return new LookupResult(LookupStatus.PendingUser, pending.Role);

        return new LookupResult(LookupStatus.Unknown, null);
    }
}
