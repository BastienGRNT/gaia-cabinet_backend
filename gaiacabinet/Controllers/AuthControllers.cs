using gaiacabinet_api.DTOs;
using gaiacabinet_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace gaiacabinet_api.Controllers;

[ApiController, Route("/api/v1/auth")]
public class AuthControllers : ControllerBase
{
    private readonly AuthServices _asvc;
    public AuthControllers(AuthServices asvc) => _asvc = asvc;

    [HttpPost("lookup")]
    public async Task<IActionResult> Lookup([FromBody] LookupDto dto)
        => Ok(new { status = await _asvc.Lookup(dto.Mail) });
}