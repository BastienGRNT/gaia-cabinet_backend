using System.Net;

namespace GaiaSolution.Domain.Exceptions;

public abstract class AppException(
    HttpStatusCode status,
    string message,
    string errorCode,
    IReadOnlyDictionary<string, object?>? details = null,
    string? field = null,
    Exception? inner = null
) : Exception(message, inner)
{
    /// <summary>Code HTTP associé à l’exception.</summary>
    public HttpStatusCode Status { get; } = status;

    /// <summary>Code fonctionnel stable destiné au client.</summary>
    public string ErrorCode { get; } = errorCode;

    /// <summary>Détails additionnels exposés dans la réponse (optionnel).</summary>
    public IReadOnlyDictionary<string, object?>? Details { get; } = details;

    /// <summary>Champ concerné par l’erreur (pour la validation, optionnel).</summary>
    public string? Field { get; } = field;
}