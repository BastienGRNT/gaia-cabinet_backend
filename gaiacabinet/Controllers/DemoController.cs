namespace gaiacabinet_api.Controllers;

// DemoController.cs
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/demo")]
public sealed class DemoController : ControllerBase
{
    private readonly DemoService _svc;
    public DemoController(DemoService svc) => _svc = svc;

    // GET /api/demo/{scenario}
    [HttpGet("{scenario:int}")]
    public async Task<ActionResult<OkResponse>> Get(int scenario, CancellationToken ct)
        => Ok(await _svc.RunAsync(scenario, ct));
}