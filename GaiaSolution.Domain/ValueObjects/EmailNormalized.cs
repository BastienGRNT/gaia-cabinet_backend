using System.Net.Mail;
using GaiaSolution.Domain.Exceptions;

namespace GaiaSolution.Domain.ValueObjects;

public sealed class EmailNormalized
{
    public string Value { get; }

    private EmailNormalized(string value) => Value = value;

    public static EmailNormalized From(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidEmailException("Email requis.",
                details: new Dictionary<string, object?> { ["reason"] = "Email vide" });

        var normalized = email.Trim().ToLowerInvariant();

        try { _ = new MailAddress(normalized); }
        catch
        {
            throw new InvalidEmailException("Format d'email invalide.",
                details: new Dictionary<string, object?> { ["reason"] = "Format d'email invalide" });
        }

        return new EmailNormalized(normalized);
    }

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is EmailNormalized other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}