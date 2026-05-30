using Api.Modules.Collections.Domain.Enums;

namespace Api.Modules.Collections.Application.Dtos;

public sealed record CreateCollectionDto(
    Guid CustomerId,
    string SenderName,
    string SenderAddress,
    string RecipientName,
    string RecipientAddress,
    DateOnly ExpectedPickupDate,
    CollectionPriority? Priority,
    string? Notes);

public sealed record CollectionSummaryDto(
    Guid Id,
    string Number,
    Guid CustomerId,
    string CustomerName,
    string SenderName,
    string RecipientName,
    DateOnly ExpectedPickupDate,
    CollectionPriority Priority,
    CollectionStatus Status,
    string? DriverName,
    string? VehiclePlate,
    bool Overdue);

public sealed record CollectionDetailsDto(
    Guid Id,
    string Number,
    Guid CustomerId,
    string CustomerName,
    string SenderName,
    string SenderAddress,
    string RecipientName,
    string RecipientAddress,
    DateOnly ExpectedPickupDate,
    CollectionPriority Priority,
    string? Notes,
    CollectionStatus Status,
    Guid? DriverId,
    string? DriverName,
    Guid? VehicleId,
    string? VehiclePlate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? AssignedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CollectedAt,
    DateTimeOffset? CancelledAt,
    string? CancellationReason,
    bool Overdue,
    IReadOnlyList<IncidentDto> Incidents);

public sealed record IncidentDto(
    Guid Id,
    string Description,
    string ResponsibleUser,
    DateTimeOffset RegisteredAt);

public sealed record OptionDto(Guid Id, string Name);

public sealed record VehicleOptionDto(Guid Id, string Plate, string Description);
