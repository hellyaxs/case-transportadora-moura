using Api.Modules.Collections.Application.Dtos;
using Api.Modules.Collections.Domain.Entities;

namespace Api.Modules.Collections.Application.UseCases;

internal static class CollectionDtoMapper
{
    public static CollectionSummaryDto ToSummary(Collection collection, DateOnly today)
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

    public static CollectionDetailsDto ToDetails(Collection collection, DateOnly today)
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
