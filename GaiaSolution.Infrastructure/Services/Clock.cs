using GaiaSolution.Application.Base.Interfaces;

namespace GaiaSolution.Infrastructure.Services;

public sealed class Clock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}