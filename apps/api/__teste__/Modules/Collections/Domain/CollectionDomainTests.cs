using Api.Modules.Collections.Domain.Entities;
using Api.Modules.Collections.Domain.Enums;
using Api.Shared.Domain.Exceptions;

namespace Api.Test.Modules.Collections.Domain;

public class CollectionDomainTests
{
    [Fact]
    public void CreateCollection_ShouldStartOpen()
    {
        var collection = CreateCollection();

        Assert.Equal(CollectionStatus.Open, collection.Status);
        Assert.Equal(CollectionPriority.Normal, collection.Priority);
        Assert.NotEqual(Guid.Empty, collection.Id);
    }

    [Fact]
    public void OpenCollectionWithAssignment_ShouldMoveToInProgressAndCollected()
    {
        var collection = CreateCollection();

        collection.AssignDriverAndVehicle(Guid.NewGuid(), "Ana Souza", Guid.NewGuid(), "abc1d23", Now());
        collection.MarkInProgress(Now().AddMinutes(10));
        collection.MarkAsCollected(Now().AddMinutes(30));

        Assert.Equal(CollectionStatus.Collected, collection.Status);
        Assert.NotNull(collection.AssignedAt);
        Assert.NotNull(collection.StartedAt);
        Assert.NotNull(collection.CollectedAt);
    }

    [Fact]
    public void CancelledCollection_ShouldNotReturnToInProgress()
    {
        var collection = CreateCollection();

        collection.Cancel("Sender unavailable", Now());

        var exception = Assert.Throws<BusinessRuleException>(() => collection.MarkInProgress(Now().AddMinutes(1)));
        Assert.Equal("collection_cancelled_terminal", exception.Code);
    }

    [Fact]
    public void CancelledCollection_ShouldNotReturnToCollected()
    {
        var collection = CreateCollection();

        collection.Cancel("Incorrect address", Now());

        var exception = Assert.Throws<BusinessRuleException>(() => collection.MarkAsCollected(Now().AddMinutes(1)));
        Assert.Equal("collection_cancelled_terminal", exception.Code);
    }

    [Fact]
    public void CollectionWithoutDriver_ShouldNotBeCollected()
    {
        var collection = CreateCollection();

        var exception = Assert.Throws<BusinessRuleException>(() => collection.MarkAsCollected(Now()));
        Assert.Equal("driver_required", exception.Code);
    }

    [Fact]
    public void CollectionWithoutVehicle_ShouldNotBeCollected()
    {
        var collection = CreateCollection();
        SetPrivate(collection, nameof(Collection.DriverId), Guid.NewGuid());
        SetPrivate(collection, nameof(Collection.DriverName), "Ana Souza");
        SetPrivate(collection, nameof(Collection.Status), CollectionStatus.InProgress);

        var exception = Assert.Throws<BusinessRuleException>(() => collection.MarkAsCollected(Now()));
        Assert.Equal("vehicle_required", exception.Code);
    }

    [Fact]
    public void CollectionWithoutAssignment_ShouldNotMoveToInProgress()
    {
        var collection = CreateCollection();

        var exception = Assert.Throws<BusinessRuleException>(() => collection.MarkInProgress(Now()));
        Assert.Equal("driver_required", exception.Code);
    }

    [Fact]
    public void RegisterIncident_ShouldStoreTimestampAndResponsibleUser()
    {
        var collection = CreateCollection();
        var registeredAt = Now();

        collection.RegisterIncident("Sender absent at location.", "operator.test", registeredAt);

        var incident = Assert.Single(collection.Incidents);
        Assert.Equal("Sender absent at location.", incident.Description);
        Assert.Equal("operator.test", incident.ResponsibleUser);
        Assert.Equal(registeredAt, incident.RegisteredAt);
    }

    private static Collection CreateCollection(CollectionPriority priority = CollectionPriority.Normal)
    {
        return new Collection(
            Guid.NewGuid(),
            "COL-TEST",
            Guid.NewGuid(),
            "Test Customer",
            "Test Sender",
            "Origin Street, 100",
            "Test Recipient",
            "Destination Street, 200",
            new DateOnly(2026, 5, 28),
            priority,
            "Notes",
            Now());
    }

    private static DateTimeOffset Now()
    {
        return new DateTimeOffset(2026, 5, 28, 12, 0, 0, TimeSpan.Zero);
    }

    private static void SetPrivate<T>(Collection collection, string propertyName, T value)
    {
        typeof(Collection).GetProperty(propertyName)!.SetValue(collection, value);
    }
}
