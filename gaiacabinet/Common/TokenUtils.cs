using System.Security.Cryptography;
using System.Text;

namespace gaiacabinet_api.Common;

public class TokenUtils
{
    public static string Base64UrlEncode(ReadOnlySpan<byte> bytes)
        => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    
    public static string HashToken(string token)
    {
        var data = Encoding.UTF8.GetBytes(token);
        return Convert.ToHexString(SHA256.HashData(data));
    }
}