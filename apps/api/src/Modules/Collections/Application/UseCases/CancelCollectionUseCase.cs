using Api.Modules.Collections.Application.Contracts;
using Api.Modules.Collections.Application.Dtos;
using Microsoft.Extensions.Logging;

namespace Api.Modules.Collections.Application.UseCases;

public sealed class CancelCollectionUseCase(
    ICollectionRepository repository,
    IClock clock,
    ILogger<CancelCollectionUseCase> logger)
{
    public async Task<CollectionDetailsDto> ExecuteAsync(Guid id, string? reason, CancellationToken cancellationToken)
    {
        var collection = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Collection not found.");

        collection.Cancel(reason ?? "Operational cancellation", clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogWarning(
            "Collection {Number} ({Id}) cancelled. Reason: {Reason}",
            collection.Number, collection.Id, reason);

        return CollectionDtoMapper.ToDetails(collection, clock.Today);
    }
}
