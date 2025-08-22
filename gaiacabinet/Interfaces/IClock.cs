namespace gaiacabinet_api.Interfaces;

// Horloge injectable (meilleure testabilité que DateTimeOffset.UtcNow)
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}