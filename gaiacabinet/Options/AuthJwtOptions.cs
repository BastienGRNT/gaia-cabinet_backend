namespace gaiacabinet_api.Options;

public sealed class AuthJwtOptions
{
    public string Issuer { get; init; } = "http://localhost:5111";
    public string Audience { get; init; } = "http://localhost:3000";
    public string SigningKey { get; init; } = "UNE_CLE_TRES_LONGUE_ET_ALEATOIRE_MIN_32_CHARS";
    public int AccessTokenMinutes { get; init; } = 15;
    public int RefreshTokenDays { get; init; } = 30;
}