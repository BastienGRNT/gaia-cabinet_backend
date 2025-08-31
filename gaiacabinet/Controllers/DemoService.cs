// DemoService.cs
using System.Net;
using gaiacabinet_api.Contracts.Exceptions;

namespace gaiacabinet_api.Controllers;

public sealed class DemoService
{
    public Task<OkResponse> RunAsync(int scenario, CancellationToken ct)
    {
        switch (scenario)
        {
            case 1:
                throw AppException.BadRequest("Requête invalide", new { field = "email" }, "bad_request");

            case 2:
                throw new AppException(HttpStatusCode.Unauthorized, "Identifiants invalides", "invalid_credentials");

            case 3:
                throw AppException.NotFound("Ressource introuvable", errorCode: "user_not_found");
            
            case 5:
                return Task.FromResult(new OkResponse(true, "Tout va bien", scenario));

            default:
                return Task.FromResult(new OkResponse(true, "Par défaut (pas d’erreur)", scenario));
        }
    }
}

public sealed record OkResponse(bool Ok, string Message, int Scenario);