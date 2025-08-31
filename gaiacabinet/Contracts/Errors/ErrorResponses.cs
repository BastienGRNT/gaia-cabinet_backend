namespace gaiacabinet_api.Contracts.Errors;

public sealed record ErrorResponse(
    string ErrorCode,
    string Error,
    object? Details
);