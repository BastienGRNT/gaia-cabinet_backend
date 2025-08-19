using gaiacabinet_api.Contracts;
using gaiacabinet_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace gaiacabinet_api.Controllers;

[ApiController, Route("/api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthServices _auth;
    public AuthController(IAuthServices  auth) => _auth = auth;

    [HttpPost("lookup")]
    [ProducesResponseType(typeof(LookupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LookupAsync([FromBody] LookupRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Email))
            return ValidationProblem(ModelState);
        
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
}