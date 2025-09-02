using System.Net;

namespace GaiaSolution.Domain.Exceptions;

public sealed class InvalidEmailException(
    string message = "Email invalide.",
    string? field = "email",
    IReadOnlyDictionary<string, object?>? details = null,
    Exception? inner = null
) : AppException(HttpStatusCode.BadRequest, message, "invalid_email", details, field, inner);