namespace Api.Application.Coletas.Contracts;

public interface IClock
{
    DateTimeOffset Now { get; }
    DateOnly Today { get; }
}
