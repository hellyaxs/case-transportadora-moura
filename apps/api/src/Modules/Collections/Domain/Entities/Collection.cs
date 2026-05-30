using Api.Modules.Collections.Domain.Enums;
using Api.Shared.Domain.Exceptions;

namespace Api.Modules.Collections.Domain.Entities;

public sealed class Collection
{
    private readonly List<CollectionIncident> _incidents = [];

    private Collection()
    {
    }

    public Collection(
        Guid id,
        string number,
        Guid customerId,
        string customerName,
        string senderName,
        string senderAddress,
        string recipientName,
        string recipientAddress,
        DateOnly expectedPickupDate,
        CollectionPriority priority,
        string? notes,
        DateTimeOffset createdAt)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Number = Required(number, "Collection number is required.");
        CustomerId = customerId == Guid.Empty ? throw new ArgumentException("Customer is required.", nameof(customerId)) : customerId;
        CustomerName = Required(customerName, "Customer name is required.");
        SenderName = Required(senderName, "Sender name is required.");
        SenderAddress = Required(senderAddress, "Sender address is required.");
        RecipientName = Required(recipientName, "Recipient name is required.");
        RecipientAddress = Required(recipientAddress, "Recipient address is required.");
        ExpectedPickupDate = expectedPickupDate;
        Priority = priority == default ? CollectionPriority.Normal : priority;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        Status = CollectionStatus.Open;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string SenderName { get; private set; } = string.Empty;
    public string SenderAddress { get; private set; } = string.Empty;
    public string RecipientName { get; private set; } = string.Empty;
    public string RecipientAddress { get; private set; } = string.Empty;
    public DateOnly ExpectedPickupDate { get; private set; }
    public CollectionPriority Priority { get; private set; }
    public string? Notes { get; private set; }
    public CollectionStatus Status { get; private set; }
    public Guid? DriverId { get; private set; }
    public string? DriverName { get; private set; }
    public Guid? VehicleId { get; private set; }
    public string? VehiclePlate { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? AssignedAt { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CollectedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }
    public IReadOnlyCollection<CollectionIncident> Incidents => _incidents.AsReadOnly();

    public bool IsOverdue(DateOnly today)
    {
        return (Status is CollectionStatus.Open or CollectionStatus.InProgress) && ExpectedPickupDate < today;
    }

    public void AssignDriverAndVehicle(
        Guid driverId,
        string driverName,
        Guid vehicleId,
        string vehiclePlate,
        DateTimeOffset assignedAt)
    {
        EnsureActiveFlow("Cannot assign a driver or vehicle to a cancelled collection.");

        if (driverId == Guid.Empty || string.IsNullOrWhiteSpace(driverName))
        {
            throw new BusinessRuleException("Driver is required to assign the collection.", "driver_required");
        }

        if (vehicleId == Guid.Empty || string.IsNullOrWhiteSpace(vehiclePlate))
        {
            throw new BusinessRuleException("Vehicle is required to assign the collection.", "vehicle_required");
        }

        DriverId = driverId;
        DriverName = driverName.Trim();
        VehicleId = vehicleId;
        VehiclePlate = vehiclePlate.Trim().ToUpperInvariant();
        AssignedAt = assignedAt;
    }

    public void MarkInProgress(DateTimeOffset startedAt)
    {
        EnsureActiveFlow("A cancelled collection cannot return to InProgress.");
        EnsureAssignmentComplete();

        if (Status != CollectionStatus.Open)
        {
            throw new BusinessRuleException("Only open collections can start execution.", "invalid_status_transition");
        }

        Status = CollectionStatus.InProgress;
        StartedAt = startedAt;
    }

    public void MarkAsCollected(DateTimeOffset collectedAt)
    {
        EnsureActiveFlow("A cancelled collection cannot return to Collected.");
        EnsureAssignmentComplete();

        if (Status != CollectionStatus.InProgress)
        {
            throw new BusinessRuleException("Only in-progress collections can be completed.", "invalid_status_transition");
        }

        Status = CollectionStatus.Collected;
        CollectedAt = collectedAt;
    }

    public void Cancel(string reason, DateTimeOffset cancelledAt)
    {
        if (Status == CollectionStatus.Cancelled)
        {
            throw new BusinessRuleException("Collection is already cancelled.", "collection_already_cancelled");
        }

        if (Status == CollectionStatus.Collected)
        {
            throw new BusinessRuleException("A collected collection cannot be cancelled.", "collection_already_collected");
        }

        Status = CollectionStatus.Cancelled;
        CancellationReason = string.IsNullOrWhiteSpace(reason) ? "Operational cancellation" : reason.Trim();
        CancelledAt = cancelledAt;
    }

    public void RegisterIncident(string description, string responsibleUser, DateTimeOffset registeredAt)
    {
        _incidents.Add(new CollectionIncident(Guid.NewGuid(), Id, description, responsibleUser, registeredAt));
    }

    private void EnsureActiveFlow(string message)
    {
        if (Status == CollectionStatus.Cancelled)
        {
            throw new BusinessRuleException(message, "collection_cancelled_terminal");
        }

        if (Status == CollectionStatus.Collected)
        {
            throw new BusinessRuleException("A collected collection cannot return to the active flow.", "collection_collected_terminal");
        }
    }

    private void EnsureAssignmentComplete()
    {
        if (DriverId is null)
        {
            throw new BusinessRuleException("Cannot advance without an assigned driver.", "driver_required");
        }

        if (VehicleId is null)
        {
            throw new BusinessRuleException("Cannot advance without an assigned vehicle.", "vehicle_required");
        }
    }

    private static string Required(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleException(message, "required_field");
        }

        return value.Trim();
    }
}
