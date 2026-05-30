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
    string? Notes)
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
            Notes);
    }
}

public sealed record AssignCollectionRequest(Guid DriverId, Guid VehicleId);

public sealed record CancelCollectionRequest(string? Reason);

public sealed record RegisterIncidentRequest(string Description);
