namespace gaiacabinet_api.Contracts.Errors;

public sealed class ApiErrorResponse
{
    public ApiError Error { get; init; } = new();
    public IEnumerable<ApiErrorDetail>? Details { get; init; }
    public string? TraceId { get; init; }
}

public sealed class ApiError
{
    public string Code { get; init; } = "";
    public string Message { get; init; } = "";
}

public sealed class ApiErrorDetail
{
    public string Field { get; init; } = "";
    public string Message { get; init; } = "";
}