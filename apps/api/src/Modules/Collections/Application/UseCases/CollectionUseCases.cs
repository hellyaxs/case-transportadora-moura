using Api.Modules.Collections.Application.Contracts;
using Api.Modules.Collections.Application.Dtos;
using Api.Modules.Collections.Domain.Entities;
using Api.Modules.Collections.Domain.Enums;
using Api.Shared.Application.Contracts;
using Api.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Api.Modules.Collections.Application.UseCases;

public sealed class CollectionUseCases(
    ICollectionRepository repository,
    ICollectionNumberGenerator numberGenerator,
    IClock clock,
    ICurrentUser currentUser,
    ILogger<CollectionUseCases> logger)
{
    public async Task<CollectionDetailsDto> CreateAsync(CreateCollectionDto input, CancellationToken cancellationToken)
    {
        var customer = await repository.GetCustomerAsync(input.CustomerId, cancellationToken)
            ?? throw new BusinessRuleException("Customer was not found.", "customer_not_found");
        var driver = await repository.GetDriverAsync(input.DriverId, cancellationToken)
            ?? throw new BusinessRuleException("Driver was not found.", "driver_not_found");
        var vehicle = await repository.GetVehicleAsync(input.VehicleId, cancellationToken)
            ?? throw new BusinessRuleException("Vehicle was not found.", "vehicle_not_found");

        var collectionNumber = numberGenerator.Generate();
        var collection = new Collection(
            Guid.NewGuid(),
            collectionNumber,
            customer.Id,
            customer.Name,
            input.SenderName,
            input.SenderAddress,
            input.RecipientName,
            input.RecipientAddress,
            input.ExpectedPickupDate,
            input.Priority ?? CollectionPriority.Normal,
            input.Notes,
            clock.Now);

        collection.AssignDriverAndVehicle(driver.Id, driver.Name, vehicle.Id, vehicle.Plate, clock.Now);

        await repository.AddAsync(collection, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Collection {Number} ({Id}) created for customer {CustomerId} with driver {DriverId} and vehicle {VehicleId}",
            collectionNumber, collection.Id, customer.Id, driver.Id, vehicle.Id);

        return ToDetails(collection, clock.Today);
    }

    public async Task<PaginatedCollectionResponseDto> ListAsync(
        CollectionStatus? status,
        Guid? customerId,
        DateOnly? startDate,
        DateOnly? endDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
        {
            throw new BusinessRuleException("Start date cannot be greater than end date.", "invalid_date_range");
        }

        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize switch
        {
            < 1 => 10,
            > 50 => 50,
            _ => pageSize,
        };

        var totalCount = await repository.CountAsync(status, customerId, startDate, endDate, cancellationToken);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)normalizedPageSize);
        var effectivePage = totalPages == 0 ? 1 : Math.Min(normalizedPage, totalPages);

        var collections = await repository.ListPagedAsync(
            status,
            customerId,
            startDate,
            endDate,
            effectivePage,
            normalizedPageSize,
            cancellationToken);
        var metrics = await repository.GetListMetricsAsync(
            status,
            customerId,
            startDate,
            endDate,
            clock.Today,
            cancellationToken);

        return new PaginatedCollectionResponseDto(
            collections.Select(collection => ToSummary(collection, clock.Today)).ToList(),
            effectivePage,
            normalizedPageSize,
            totalCount,
            totalPages,
            metrics);
    }

    public async Task<CollectionDetailsDto> GetDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await GetCollectionAsync(id, cancellationToken);
        return ToDetails(collection, clock.Today);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await GetCollectionAsync(id, cancellationToken);

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

    public async Task<CollectionDetailsDto> MarkInProgressAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await GetCollectionAsync(id, cancellationToken);
        collection.MarkInProgress(clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDetails(collection, clock.Today);
    }

    public async Task<CollectionDetailsDto> CompleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await GetCollectionAsync(id, cancellationToken);
        collection.MarkAsCollected(clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDetails(collection, clock.Today);
    }

    public async Task<CollectionDetailsDto> CancelAsync(Guid id, string? reason, CancellationToken cancellationToken)
    {
        var collection = await GetCollectionAsync(id, cancellationToken);
        collection.Cancel(reason ?? "Operational cancellation", clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogWarning(
            "Collection {Number} ({Id}) cancelled. Reason: {Reason}",
            collection.Number, collection.Id, reason);

        return ToDetails(collection, clock.Today);
    }

    public async Task<CollectionDetailsDto> RegisterIncidentAsync(
        Guid id,
        string description,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(currentUser.DisplayName))
        {
            throw new BusinessRuleException("Authenticated user is required.", "authenticated_user_required");
        }

        var collection = await GetCollectionAsync(id, cancellationToken);
        collection.RegisterIncident(description, currentUser.DisplayName, clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogWarning(
            "Incident registered on collection {Number} ({Id}) by {User}: {Description}",
            collection.Number, collection.Id, currentUser.DisplayName, description);

        return ToDetails(collection, clock.Today);
    }

    private async Task<Collection> GetCollectionAsync(Guid id, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Collection not found.");
    }

    private static CollectionSummaryDto ToSummary(Collection collection, DateOnly today)
    {
        return new CollectionSummaryDto(
            collection.Id,
            collection.Number,
            collection.CustomerId,
            collection.CustomerName,
            collection.SenderName,
            collection.RecipientName,
            collection.ExpectedPickupDate,
            collection.Priority,
            collection.Status,
            collection.DriverName,
            collection.VehiclePlate,
            collection.IsOverdue(today));
    }

    private static CollectionDetailsDto ToDetails(Collection collection, DateOnly today)
    {
        return new CollectionDetailsDto(
            collection.Id,
            collection.Number,
            collection.CustomerId,
            collection.CustomerName,
            collection.SenderName,
            collection.SenderAddress,
            collection.RecipientName,
            collection.RecipientAddress,
            collection.ExpectedPickupDate,
            collection.Priority,
            collection.Notes,
            collection.Status,
            collection.DriverId,
            collection.DriverName,
            collection.VehicleId,
            collection.VehiclePlate,
            collection.CreatedAt,
            collection.AssignedAt,
            collection.StartedAt,
            collection.CollectedAt,
            collection.CancelledAt,
            collection.CancellationReason,
            collection.IsOverdue(today),
            collection.Incidents
                .OrderBy(incident => incident.RegisteredAt)
                .Select(incident => new IncidentDto(
                    incident.Id,
                    incident.Description,
                    incident.ResponsibleUser,
                    incident.RegisteredAt))
                .ToList());
    }
}
