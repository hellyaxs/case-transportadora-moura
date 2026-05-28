using Api.Application.Coletas.Contracts;

namespace Api.Infrastructure.Coletas;

public sealed class ColetaNumeroGenerator : IColetaNumeroGenerator
{
    public string Gerar()
    {
        return $"COL-{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}";
    }
}
