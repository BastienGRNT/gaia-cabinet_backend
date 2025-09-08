using System.Net;

namespace GaiaSolution.Domain.Exceptions;

public class InvalidAddException(
    string message = "Impossible de récupérer l'entité",
    string? field = null,
    IReadOnlyDictionary<string, object?>? details = null,
    Exception? inner = null
) : AppException(HttpStatusCode.NotFound, message, "entities_not_found", details, field, inner);