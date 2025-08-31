using System.Security.Cryptography;
using System.Text;
using gaiacabinet_api.Options;
using Microsoft.Extensions.Options;

namespace gaiacabinet_api.Common;

public class HashUtils
{
    private readonly string _pepper;

    public HashUtils(IOptions<OtpPepperOptions> otpOptions)
    {
        _pepper = otpOptions.Value.OtpPepper;
    }

    public static string Base64UrlEncode(ReadOnlySpan<byte> bytes)
        => Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

    public string HashString(string token)
    {
        var data = Encoding.UTF8.GetBytes(token + _pepper);
        return Convert.ToHexString(SHA256.HashData(data));
    }
}