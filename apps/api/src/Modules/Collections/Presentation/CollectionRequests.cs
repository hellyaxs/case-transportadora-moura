using Api.Modules.Collections.Application.Dtos;
using Api.Modules.Collections.Domain.Enums;

namespace Api.Modules.Collections.Presentation;

public sealed record CreateCollectionRequest(
    Guid CustomerId,
    string SenderName,
    string SenderAddress,
    string RecipientName,
    string RecipientAddress,
    DateOnly ExpectedPickupDate,
    CollectionPriority? Priority,
    string? Notes,
    Guid DriverId,
    Guid VehicleId)
{
    public CreateCollectionDto ToDto()
    {
        return new CreateCollectionDto(
            CustomerId,
            SenderName,
            SenderAddress,
            RecipientName,
            RecipientAddress,
            ExpectedPickupDate,
            Priority,
            Notes,
            DriverId,
            VehicleId);
    }
}

public sealed record CancelCollectionRequest(string? Reason);

public sealed record RegisterIncidentRequest(string Description);
