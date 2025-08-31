using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using gaiacabinet_api.Contracts.Errors;
using gaiacabinet_api.Contracts.Exceptions;

namespace gaiacabinet_api.Middlewares;

public sealed class ErrorHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        // camelCase + n'écrit pas les nulls
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            var status = (int)ex.Status;

            if (status >= StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "{ErrorCode}: {Message}", ex.ErrorCode, ex.Message);
            else
                _logger.LogWarning("{ErrorCode}: {Message}", ex.ErrorCode, ex.Message);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response already started; cannot write error body.");
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = status;          // <-- code HTTP (non répété dans le body)
            context.Response.ContentType = "application/json";

            var payload = new ErrorResponse(
                ex.ErrorCode,
                ex.Message,
                ex.Details
            );

            await context.Response.WriteAsJsonAsync(payload, JsonOptions);
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            _logger.LogError(ex, "Unhandled exception traceId={TraceId}", traceId);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response already started; cannot write error body.");
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new ErrorResponse(
                "internal_error",
                "Une erreur inattendue est survenue.",
                new { traceId }
            );

            await context.Response.WriteAsJsonAsync(payload, JsonOptions);
        }
    }
} 