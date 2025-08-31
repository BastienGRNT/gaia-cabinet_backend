using System.Net;

namespace gaiacabinet_api.Contracts.Exceptions;

public sealed class AppException : Exception
{
    public HttpStatusCode Status { get; }
    public string ErrorCode { get; }
    public object? Details { get; }

    public AppException(HttpStatusCode status, string message, string errorCode, object? details = null)
        : base(message)
    {
        Status = status;
        ErrorCode = errorCode;
        Details = details;
    }

    public static AppException BadRequest(string message, object? details = null, string errorCode = "bad_request")
        => new(HttpStatusCode.BadRequest, message, errorCode, details);

    public static AppException NotFound(string message, object? details = null, string errorCode = "not_found")
        => new(HttpStatusCode.NotFound, message, errorCode, details);
    
    public static AppException Unauthorized(string message = "Unauthorized", object? details = null, string errorCode = "unauthorized")
        => new(HttpStatusCode.Unauthorized, message, errorCode, details);

    public static AppException Forbidden(string message = "Forbidden", object? details = null, string errorCode = "forbidden")
        => new(HttpStatusCode.Forbidden, message, errorCode, details);
}