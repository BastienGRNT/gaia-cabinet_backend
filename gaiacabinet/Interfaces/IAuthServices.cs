using gaiacabinet_api.Models;

namespace gaiacabinet_api.Interfaces;

public interface IAuthServices
{
    Task<LookupResult> LookupAsync(string email, CancellationToken ct);
    Task<LoginResult> LoginAsync(string email, string password, string ip, string userAcces, CancellationToken ct);
    Task<RefreshResult> RefreshAsync(string refreshToken, string sessionKey, string ip, string userAgent, CancellationToken ct);
    Task<VerifyMailResult> VerifyMailAsync(string email, string verificationCode, CancellationToken ct);
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
    public string SessionKey;
    
    public LoginResult(string accessToken, string refreshToken, string sessionKey)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        SessionKey = sessionKey;
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

public sealed record VerifyMailResult
{
    public string Email;
    public string VerifyEmailToken;

    public VerifyMailResult(string email, string verifyEmailToken)
    {
        Email = email;
        VerifyEmailToken = verifyEmailToken;
    }
}