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

        await repository.AddAsync(collection, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Collection {Number} ({Id}) created for customer {CustomerId}",
            collectionNumber, collection.Id, customer.Id);

        return ToDetails(collection, clock.Today);
    }

    public async Task<IReadOnlyList<CollectionSummaryDto>> ListAsync(
        CollectionStatus? status,
        Guid? customerId,
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken cancellationToken)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
        {
            throw new BusinessRuleException("Start date cannot be greater than end date.", "invalid_date_range");
        }

        var collections = await repository.ListAsync(status, customerId, startDate, endDate, cancellationToken);
        return collections.Select(collection => ToSummary(collection, clock.Today)).ToList();
    }

    public async Task<CollectionDetailsDto> GetDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await GetCollectionAsync(id, cancellationToken);
        return ToDetails(collection, clock.Today);
    }

    public async Task<CollectionDetailsDto> AssignAsync(Guid id, Guid driverId, Guid vehicleId, CancellationToken cancellationToken)
    {
        var collection = await GetCollectionAsync(id, cancellationToken);
        var driver = await repository.GetDriverAsync(driverId, cancellationToken)
            ?? throw new BusinessRuleException("Driver was not found.", "driver_not_found");
        var vehicle = await repository.GetVehicleAsync(vehicleId, cancellationToken)
            ?? throw new BusinessRuleException("Vehicle was not found.", "vehicle_not_found");

        collection.AssignDriverAndVehicle(driver.Id, driver.Name, vehicle.Id, vehicle.Plate, clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Collection {Number} ({Id}) assigned to driver {DriverName} ({DriverId}) with vehicle {VehiclePlate} ({VehicleId})",
            collection.Number, collection.Id, driver.Name, driver.Id, vehicle.Plate, vehicle.Id);

        return ToDetails(collection, clock.Today);
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
