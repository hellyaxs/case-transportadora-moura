using Api.Application.Coletas.Contracts;

namespace Api.Infrastructure.Coletas;

public sealed class SystemClock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(Now.UtcDateTime);
}
