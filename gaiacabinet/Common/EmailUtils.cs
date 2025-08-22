namespace gaiacabinet_api.Common;

public class EmailUtils
{
    public static string Normalize(string email) => email.Trim().ToLowerInvariant();
}