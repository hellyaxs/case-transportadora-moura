using Api.Modules.Collections.Application.Contracts;

namespace Api.Modules.Collections.Infrastructure;

public sealed class CollectionNumberGenerator : ICollectionNumberGenerator
{
    public string Generate()
    {
        return $"COL-{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}";
    }
}
