using System.Net;

namespace GaiaSolution.Domain.Exceptions;

public sealed class InvalidPhoneException(
    string message = "Numéro de télephone invalide.",
    string? field = "phone_number",
    IReadOnlyDictionary<string, object?>? details = null,
    Exception? inner = null
) : AppException(HttpStatusCode.BadRequest, message, "invalid_phone_number", details, field, inner);