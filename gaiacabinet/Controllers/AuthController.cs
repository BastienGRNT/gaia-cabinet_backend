using gaiacabinet_api.Contracts;
using gaiacabinet_api.Contracts.Errors;
using gaiacabinet_api.Interfaces;
using gaiacabinet_api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace gaiacabinet_api.Controllers;

[Produces("application/json")]
[ApiController, Route("/api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthServices _auth;
    private readonly AuthJwtOptions _jwt;

    public AuthController(IAuthServices auth, IOptions<AuthJwtOptions> jwtOptions)
    {
        _auth = auth;
        _jwt = jwtOptions.Value;
    }

    [HttpPost("lookup")]
    [ProducesResponseType(typeof(LookupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LookupAsync([FromBody] LookupRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Email))
        {
            var details = ModelState
                .Where(kv => kv.Value?.Errors.Count > 0)
                .Select(kv => new ApiErrorDetail { Field = kv.Key, Message = kv.Value!.Errors[0].ErrorMessage })
                .ToList();

            return BadRequest(new ApiErrorResponse
            {
                Error = new ApiError { Code = "validation_failed", Message = "Données invalides." },
                Details = details,
                TraceId = HttpContext.TraceIdentifier
            });
        }
        
        var result = await _auth.LookupAsync(request.Email, ct);

        var reponse = new LookupResponse
        {
            Status = result.Status,
            Role = result.Role is null
                ? null
                : new RoleDto()
                {
                    RoleId = result.Role.RoleId,
                    Label = result.Role.Label
                }
        };
        
        return Ok(reponse);
    }
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        // Validation "front-friendly"
        if (!ModelState.IsValid)
        {
            var details = ModelState
                .Where(kv => kv.Value?.Errors.Count > 0)
                .Select(kv => new ApiErrorDetail { Field = kv.Key, Message = kv.Value!.Errors[0].ErrorMessage })
                .ToList();

            return BadRequest(new ApiErrorResponse
            {
                Error = new ApiError { Code = "validation_failed", Message = "Données invalides." },
                Details = details,
                TraceId = HttpContext.TraceIdentifier
            });
        }

        try
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ua = Request.Headers.UserAgent.ToString();

            var result = await _auth.LoginAsync(request.Email, request.Password, ip ?? string.Empty, ua, ct);

            // Cookie refresh token (HttpOnly)
            Response.Cookies.Append("rt", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                // Si front sur un autre domaine : SameSite=None (et HTTPS obligatoire)
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(_jwt.RefreshTokenDays),
                Path = "/api/v1/auth"
            });
            
            Response.Cookies.Append("skey", result.SessionKey, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(_jwt.RefreshTokenDays),
                Path = "/api/v1/auth"
            });

            return Ok(new LoginResponse
            {
                AccessToken = result.AccessToken,
                TokenType = "Bearer"
            });
        }
        catch (UnauthorizedAccessException ex) when (ex.Message == "invalid_credentials")
        {
            return Unauthorized(new ApiErrorResponse
            {
                Error = new ApiError { Code = "invalid_credentials", Message = "Identifiants invalides." },
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (UnauthorizedAccessException ex) when (ex.Message == "not_authorized")
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiErrorResponse
            {
                Error = new ApiError { Code = "forbidden", Message = "Accès refusé." },
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Error = new ApiError { Code = "server_error", Message = "Une erreur est survenue." },
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Verify(CancellationToken ct)
    {
        var rt = Request.Cookies["rt"];
        if (string.IsNullOrWhiteSpace(rt))
            return Unauthorized(new ApiErrorResponse {
                Error = new ApiError { Code = "invalid_refresh_token", Message = "Refresh token manquant." },
                TraceId = HttpContext.TraceIdentifier
            });
        
        var skey = Request.Cookies["skey"];
        if (string.IsNullOrWhiteSpace(skey))
            return Unauthorized(new ApiErrorResponse {
                Error = new ApiError { Code = "invalid_session", Message = "Session key manquant." },
                TraceId = HttpContext.TraceIdentifier
            });
        
        try
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ua = Request.Headers.UserAgent.ToString();

            var result = await _auth.RefreshAsync(rt, skey, ip ?? string.Empty, ua, ct);

            // 2) Poser le **nouveau** refresh token (rotation)
            Response.Cookies.Append("rt", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax, // si front ≠ domaine: None + HTTPS
                Expires = DateTimeOffset.UtcNow.AddDays(_jwt.RefreshTokenDays),
                Path = "/api/v1/auth"
            });

            return Ok(new LoginResponse { AccessToken = result.AccessToken, TokenType = "Bearer" });
        }
        catch (UnauthorizedAccessException ex) when (ex.Message is "invalid_refresh_token" or "expired_refresh_token" or "revoked_refresh_token" or "invalid_session_key")
        {
            Response.Cookies.Delete("rt", new CookieOptions { Path = "/api/v1/auth" });
            Response.Cookies.Delete("skey", new CookieOptions { Path = "/api/v1/auth" });

            return Unauthorized(new ApiErrorResponse {
                Error = new ApiError { Code = ex.Message, Message = "Refresh token invalide." },
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (UnauthorizedAccessException ex) when (ex.Message == "not_authorized")
        {
            Response.Cookies.Delete("rt", new CookieOptions { Path = "/api/v1/auth" });
            Response.Cookies.Delete("skey", new CookieOptions { Path = "/api/v1/auth" });
            
            return StatusCode(StatusCodes.Status403Forbidden, new ApiErrorResponse {
                Error = new ApiError { Code = "forbidden", Message = "Accès refusé." },
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch
        {
            Response.Cookies.Delete("rt", new CookieOptions { Path = "/api/v1/auth" });
            Response.Cookies.Delete("skey", new CookieOptions { Path = "/api/v1/auth" });
            
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse {
                Error = new ApiError { Code = "server_error", Message = "Une erreur est survenue." },
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

}