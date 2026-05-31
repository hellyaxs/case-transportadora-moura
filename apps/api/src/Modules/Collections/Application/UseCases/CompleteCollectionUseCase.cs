using Api.Modules.Collections.Application.Contracts;
using Api.Modules.Collections.Application.Dtos;

namespace Api.Modules.Collections.Application.UseCases;

public sealed class CompleteCollectionUseCase(
    ICollectionRepository repository,
    IClock clock)
{
    public async Task<CollectionDetailsDto> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Collection not found.");

        collection.MarkAsCollected(clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        return CollectionDtoMapper.ToDetails(collection, clock.Today);
    }
}
