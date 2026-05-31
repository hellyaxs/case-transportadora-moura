using Api.Modules.Collections.Application.Contracts;
using Api.Modules.Collections.Application.Dtos;
using Api.Shared.Application.Contracts;
using Api.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Api.Modules.Collections.Application.UseCases;

public sealed class RegisterCollectionIncidentUseCase(
    ICollectionRepository repository,
    IClock clock,
    ICurrentUser currentUser,
    ILogger<RegisterCollectionIncidentUseCase> logger)
{
    public async Task<CollectionDetailsDto> ExecuteAsync(
        Guid id,
        string description,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(currentUser.DisplayName))
        {
            throw new BusinessRuleException("Authenticated user is required.", "authenticated_user_required");
        }

        var collection = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Collection not found.");

        collection.RegisterIncident(description, currentUser.DisplayName, clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogWarning(
            "Incident registered on collection {Number} ({Id}) by {User}: {Description}",
            collection.Number, collection.Id, currentUser.DisplayName, description);

        return CollectionDtoMapper.ToDetails(collection, clock.Today);
    }
}
