using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using gaiacabinet_api.Contracts;
using gaiacabinet_api.Contracts.Errors;
using gaiacabinet_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gaiacabinet_api.Controllers;

[Produces("application/json")]
[ApiController, Route("/api/v1/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized(new ApiErrorResponse
            {
                Error = new ApiError { Code = "unauthorized", Message = "Token invalide." },
                TraceId = HttpContext.TraceIdentifier
            });
        }

        var user = await _userService.GetCurrentUserAsync(int.Parse(userId), ct);

        return Ok(user);
    }
    
}