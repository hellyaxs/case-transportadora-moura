using Api.Modules.Collections.Application.Contracts;
using Api.Modules.Collections.Application.Dtos;
using Api.Modules.Collections.Domain.Entities;
using Api.Modules.Collections.Domain.Enums;
using Api.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Api.Modules.Collections.Application.UseCases;

public sealed class CreateCollectionUseCase(
    ICollectionRepository repository,
    ICollectionNumberGenerator numberGenerator,
    IClock clock,
    ILogger<CreateCollectionUseCase> logger)
{
    public async Task<CollectionDetailsDto> ExecuteAsync(CreateCollectionDto input, CancellationToken cancellationToken)
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

        return CollectionDtoMapper.ToDetails(collection, clock.Today);
    }
}
