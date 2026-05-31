using Api.Modules.Collections.Application.Contracts;
using Api.Modules.Collections.Domain.Enums;
using Api.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Api.Modules.Collections.Application.UseCases;

public sealed class DeleteCollectionUseCase(
    ICollectionRepository repository,
    ILogger<DeleteCollectionUseCase> logger)
{
    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Collection not found.");

        if (collection.Status != CollectionStatus.Collected)
        {
            throw new BusinessRuleException(
                "Only collected collections can be deleted.",
                "collection_not_deletable");
        }

        await repository.DeleteAsync(collection, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Collection {Number} ({Id}) deleted after completion",
            collection.Number,
            collection.Id);
    }
}
