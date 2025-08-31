using gaiacabinet_api.Common;
using gaiacabinet_api.Contracts;
using gaiacabinet_api.Contracts.Exceptions;
using gaiacabinet_api.Database;
using gaiacabinet_api.Interfaces;
using gaiacabinet_api.Models;
using gaiacabinet_api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace gaiacabinet_api.Services;


public sealed class AuthServices : IAuthServices
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly AuthJwtOptions _jwt;
    private readonly ITokenService _jwtService;
    private readonly HashUtils _hash;

    public AuthServices(AppDbContext db, IClock clock, IOptions<AuthJwtOptions> jwtOptions, ITokenService jwtService, HashUtils hashUtils)
    {
        _db = db;
        _clock = clock;
        _jwt = jwtOptions.Value;
        _jwtService = jwtService;
        _hash = hashUtils;
    }
    
    // Vérifier si un mail est en attente de création d'un compte
    public async Task<LookupResult> LookupAsync(string email, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw AppException.BadRequest(errorCode: "missing_field", message: "Email requis");

        var normalized = EmailUtils.Normalize(email);
        
        // Rechercher si un utilisateur existe avec cette email
        var userExists = await _db.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == normalized, ct);
        if (userExists)
            return new LookupResult(LookupStatus.ExistingUser, null);
        
        var now = _clock.UtcNow;

        // Rechercher si un utilisateur est en attente avec cette email
        var pending = await _db.PendingUsers
            .AsNoTracking()
            .Include(p => p.Role)
            .FirstOrDefaultAsync(
                p => p.Email == normalized
                     && p.IsActive
                     && p.ExpiresAt > now
                     && p.ConsumedAt == null,
                ct);
        
        // Si aucun utilisateurs dans User et dans Pending alors return status "unknown"
        if (pending is null)
            throw AppException.NotFound(errorCode: "pending_user_not_found", message: "Aucun utilisateur en attente trouvé");

        // Vérification anti spam
        if (pending.VerificationCodeExpiration.HasValue && pending.VerificationCodeExpiration > now)
        { return new LookupResult(LookupStatus.PendingUser, pending.Role); }
        if (pending.VerificationCodeCreation.HasValue && now - pending.VerificationCodeCreation.Value < TimeSpan.FromSeconds(60))
            return new LookupResult(LookupStatus.PendingUser, pending.Role);

        var code = VerificationCodeService.GenerateVerificationCode();
        var hash = _hash.HashString(code);

        var updated = await _db.PendingUsers
            .Where(p => p.Email == normalized 
                && p.ConsumedAt == null                                     //Pas Consommé
                && p.IsActive                                               //Pending Active (pas révoquée)
                && p.ExpiresAt > now                                        //Pending pas Expirée 
                && p.VerificationCodeCreation == null)                      //Pas de Code déja envoyer par mail // TODO : Ajouter un endpoint pas de mail recu -> Renvoyer un code et mettre a jour la table
            .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.UpdatedAt, now)                     //On MAJ la date de la last modif
                    .SetProperty(p => p.VerificationCodeHash, hash)         // Ajouter le Hash du Code de vérification envoyer par mail
                    .SetProperty(p => p.VerificationCodeCreation, now)
                    .SetProperty(p => p.VerificationCodeExpiration, now.AddMinutes(3)), 
                ct);

        if (updated > 0)
        {
            // TODO : Retirer le log en prod
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;

            Console.WriteLine("\n======================================");
            Console.WriteLine("[DEBUG-CODE] Votre code est : " + code);
            Console.WriteLine("======================================\n");
            Console.ResetColor();
            
            // TODO: envoyer le `VerificationCode` si updated > 0
        }
        
        return new LookupResult(LookupStatus.PendingUser, pending.Role);

    }
    
    public async Task<VerifyMailResult> VerifyMailAsync(string email, string verificationCode, CancellationToken ct)
    {
        var normalized = EmailUtils.Normalize(email);
        var now = _clock.UtcNow;
        
        //Rechercher l'utilisateurs qui comfirme sont email
        var pendingUser = await _db.PendingUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == normalized
                && p.ConsumedAt == null                             //Pas consommé
                && p.IsActive                                       //Pending active (pas révoquée)
                && p.ExpiresAt > now                                //Pending pas éxpirée
                && p.VerificationCodeExpiration > now               //VerificationCode pas éxpriré
                && (p.UnlockAt == null || p.UnlockAt <= now)        //Pas de bloqua
                , ct);
        
        if (pendingUser is null) throw new UnauthorizedAccessException("invalid_credentials");

        if (pendingUser.UnlockAt >= now)
        {
            var waitTime = pendingUser.UnlockAt.Value - now;
            throw new UnauthorizedAccessException("Attendre" + waitTime);
        }
        
        if (_hash.HashString(verificationCode) != pendingUser.VerificationCodeHash)
        {
            // Si le code est faux et que nombre d'essaie de vérification du mail est maintenant supérieur a 3
            if (pendingUser.Attempts + 1 >= 3)
            {
                await _db.PendingUsers
                    .Where(p => p.Email == normalized)
                    .ExecuteUpdateAsync(set => set
                        .SetProperty(s => s.UpdatedAt, now)                                     //On MAJ la date de la last modif
                        .SetProperty(s => s.Attempts, 0)                                        //On remet les Essais à O
                        .SetProperty(s => s.UnlockAt, now.AddMinutes(5))                        //On autorise le prochain essai dans 5 minutes
                        .SetProperty(s => s.VerificationCodeHash, (string?)null)                //On m'est null le VeriricationCode (Pour pas empecher dans créer un nouveau dans LookupAsync)
                        .SetProperty(s => s.VerificationCodeCreation, (DateTimeOffset?)null)    //On m'est null
                        .SetProperty(s => s.VerificationCodeExpiration, (DateTimeOffset?)null)  //On m'est null
                    , ct);
            
                throw new UnauthorizedAccessException("max_attempts_reached_wait_5_minutes");                
            }
            
            await _db.PendingUsers
                .Where(p => p.Email == normalized)
                .ExecuteUpdateAsync(set => set
                    .SetProperty(s => s.Attempts, s => s.Attempts + 1), ct);
            
            throw new UnauthorizedAccessException("invalid_verification_code");
        }
            
        // Génerer un GUID sans tiret
        var ValidateToken = Guid.NewGuid().ToString("N");
        var ValidateTokenHash = _hash.HashString(ValidateToken);

        var updated = await _db.PendingUsers
            .Where(p => p.Email == normalized           //Autres vérification faites pendant la recherche de l'utilisateur
                && p.VerificationCodeExpiration > now)  //VerificationCode pas expiré
            .ExecuteUpdateAsync(set => set
                .SetProperty(s => s.UpdatedAt, now)                                 //On MAJ la date de la last modif
                .SetProperty(s => s.Attempts, 0)                                    // Remise a zéro car si compte pas créer lookup doit pouvoir regénerer un code
                .SetProperty(s => s.ValidateTokenHash, ValidateTokenHash)           //On ajoute le Hash du Token
                .SetProperty(s => s.ValidateTokenCreation, now)
                .SetProperty(s => s.ValidateTokenExpiration, now.AddMinutes(15)), ct);
        // Consumed at a MAJ quand compte créer
        // PAS BESOIN QUE LOOKUP RENVOIE ROLE -> VERIRICATION MAIL DOIT LE FAIRE PARCONTRE
        if (updated == 0) throw new UnauthorizedAccessException("not_authorized"); 
        
        return new VerifyMailResult(pendingUser.Email, ValidateToken);
    }

    // Méthode utiliiser pour se connecter
    public async Task<LoginResult> LoginAsync(string email, string password, string ip, string userAgent, CancellationToken ct)
    {
        var normalized = EmailUtils.Normalize(email);
        
        var user = await _db.Users
            .Include(p => p.Role)
            .FirstOrDefaultAsync(u => u.Email == normalized, ct);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("invalid_credentials");
        
        if(!user.Authorized)
            throw new UnauthorizedAccessException("not_authorized");
        
        user.LastLogin = _clock.UtcNow;
        await _db.SaveChangesAsync(ct);

        var accessToken = _jwtService.GenerateAccessToken(user);

        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshHash = _hash.HashString(refreshToken);
        var refreshExpiration = _clock.UtcNow.AddDays(_jwt.RefreshTokenDays);
        var sessionKey = Guid.NewGuid().ToString("N");
        var sKeyHash = _hash.HashString(sessionKey);

        var rs = _db.RefreshSessions.Add(new RefreshSession
        {
            UserId = user.UserId,
            SessionKeyHash = sKeyHash,
            TokenHash = refreshHash,
            ExpiresAt = refreshExpiration,
            LastIp = ip,
            LastUserAgent = userAgent
        });
        await _db.SaveChangesAsync(ct);
        
        if (rs is null) throw new UnauthorizedAccessException("invalid_credentials");
        
        return new LoginResult(accessToken, refreshToken, sessionKey);
    }

    // Méthode utiliser pour Rafraichir un token
    public async Task<RefreshResult> RefreshAsync(string refreshToken, string sessionKey, string ip, string userAgent, CancellationToken ct)
    {
        var pair = await _jwtService.VerifyAndRotateAsync(refreshToken, sessionKey, ip, userAgent, ct);
        return new RefreshResult(pair.AccessToken, pair.RefreshToken);
    }

    
    
}
