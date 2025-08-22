using System.Security.Cryptography;

namespace gaiacabinet_api.Services;

public class VerificationCodeService
{
    public static string GenerateVerificationCode()
    {
        var number = RandomNumberGenerator.GetInt32(0, 1_000_000); 
        return number.ToString("D6");
    }
}