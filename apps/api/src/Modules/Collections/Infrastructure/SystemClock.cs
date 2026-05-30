using Api.Modules.Collections.Application.Contracts;

namespace Api.Modules.Collections.Infrastructure;

public sealed class SystemClock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(Now.UtcDateTime);
}
