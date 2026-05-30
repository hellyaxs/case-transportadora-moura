namespace Api.Modules.Collections.Application.Contracts;

public interface IClock
{
    DateTimeOffset Now { get; }
    DateOnly Today { get; }
}
