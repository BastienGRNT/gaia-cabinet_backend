using GaiaSolution.Application.Base.Interfaces;

namespace GaiaSolution.Infrastructure.Base;

public sealed class Clock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}