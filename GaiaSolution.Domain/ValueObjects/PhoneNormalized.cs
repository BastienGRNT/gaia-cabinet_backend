using PhoneNumbers;
using GaiaSolution.Domain.Exceptions;

namespace GaiaSolution.Domain.ValueObjects;

public class PhoneNormalized
{
    private static readonly PhoneNumberUtil _util = PhoneNumberUtil.GetInstance();
    
    public string Value { get; }
    
    private PhoneNormalized(string value) => Value = value;

    public static PhoneNormalized From(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new InvalidPhoneException("Numéro de telophone requis.");

        var parsed = _util.Parse(phone, "FR");

        if (!_util.IsValidNumberForRegion(parsed, "FR"))
            throw new InvalidPhoneException("Numéro invalide pour la France.");

        var normalized = _util.Format(parsed, PhoneNumberFormat.E164);

        return new PhoneNormalized(normalized);
    }
}