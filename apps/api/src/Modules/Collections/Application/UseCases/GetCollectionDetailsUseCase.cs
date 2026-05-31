using Api.Modules.Collections.Application.Contracts;
using Api.Modules.Collections.Application.Dtos;

namespace Api.Modules.Collections.Application.UseCases;

public sealed class GetCollectionDetailsUseCase(
    ICollectionRepository repository,
    IClock clock)
{
    public async Task<CollectionDetailsDto> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Collection not found.");

        return CollectionDtoMapper.ToDetails(collection, clock.Today);
    }
}
