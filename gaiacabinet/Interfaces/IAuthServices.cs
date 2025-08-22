using gaiacabinet_api.Models;

namespace gaiacabinet_api.Interfaces;

public interface IAuthServices
{
    Task<LookupResult> LookupAsync(string email, CancellationToken ct);
    Task<LoginResult> LoginAsync(string email, string password, string ip, string userAcces, CancellationToken ct);
    Task<RefreshResult> RefreshAsync(string refreshToken, string ip, string userAgent, CancellationToken ct);
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