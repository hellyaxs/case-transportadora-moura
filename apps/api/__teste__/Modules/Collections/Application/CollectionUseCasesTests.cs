using Api.Modules.Collections.Application.Contracts;
using Api.Modules.Collections.Application.Dtos;
using Api.Modules.Collections.Application.UseCases;
using Api.Modules.Collections.Domain.Entities;
using Api.Modules.Collections.Domain.Enums;
using Api.Shared.Application.Contracts;
using Api.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Api.Test.Modules.Collections.Application;

public class CollectionUseCasesTests
{
    [Fact]
    public async Task CreateAsync_ShouldPersistOpenCollection()
    {
        var repository = new FakeCollectionRepository();
        var customerId = Guid.NewGuid();
        repository.Customers[customerId] = new Customer(customerId, "Test Customer");
        var useCases = CreateUseCases(repository, fixedNow: Now());

        var result = await useCases.CreateAsync(
            new CreateCollectionDto(
                customerId,
                "Sender",
                "Street A, 1",
                "Recipient",
                "Street B, 2",
                new DateOnly(2026, 5, 28),
                CollectionPriority.High,
                "Notes"),
            CancellationToken.None);

        Assert.Equal(CollectionStatus.Open, result.Status);
        Assert.Equal("COL-0001", result.Number);
        Assert.Single(repository.Collections);
    }

    [Fact]
    public async Task CreateAsync_MissingCustomer_ShouldFail()
    {
        var useCases = CreateUseCases(new FakeCollectionRepository(), fixedNow: Now());

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCases.CreateAsync(
                new CreateCollectionDto(
                    Guid.NewGuid(),
                    "Sender",
                    "Street A, 1",
                    "Recipient",
                    "Street B, 2",
                    new DateOnly(2026, 5, 28),
                    null,
                    null),
                CancellationToken.None));

        Assert.Equal("customer_not_found", exception.Code);
    }

    [Fact]
    public async Task AssignAsync_ShouldLinkDriverAndVehicle()
    {
        var repository = new FakeCollectionRepository();
        var collection = SeedCollection(repository);
        var driverId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        repository.Drivers[driverId] = new Driver(driverId, "Ana Souza");
        repository.Vehicles[vehicleId] = new Vehicle(vehicleId, "ABC1D23", "Van");
        var useCases = CreateUseCases(repository, fixedNow: Now());

        var result = await useCases.AssignAsync(collection.Id, driverId, vehicleId, CancellationToken.None);

        Assert.Equal("Ana Souza", result.DriverName);
        Assert.Equal("ABC1D23", result.VehiclePlate);
    }

    [Fact]
    public async Task RegisterIncidentAsync_ShouldPersistResponsibleUser()
    {
        var repository = new FakeCollectionRepository();
        var collection = SeedCollection(repository);
        var useCases = CreateUseCases(repository, fixedNow: Now(), currentUser: new FakeCurrentUser("operator.test"));

        var result = await useCases.RegisterIncidentAsync(
            collection.Id,
            "Sender absent.",
            CancellationToken.None);

        var incident = Assert.Single(result.Incidents);
        Assert.Equal("operator.test", incident.ResponsibleUser);
        Assert.Equal("Sender absent.", incident.Description);
    }

    [Fact]
    public async Task CancelAsync_CancelledCollection_ShouldNotAllowAssign()
    {
        var repository = new FakeCollectionRepository();
        var collection = SeedCollection(repository);
        var useCases = CreateUseCases(repository, fixedNow: Now());
        await useCases.CancelAsync(collection.Id, "Unavailable", CancellationToken.None);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCases.AssignAsync(collection.Id, Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task ListAsync_InvalidDateRange_ShouldFail()
    {
        var useCases = CreateUseCases(new FakeCollectionRepository(), fixedNow: Now());

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCases.ListAsync(
                null,
                null,
                new DateOnly(2026, 5, 10),
                new DateOnly(2026, 5, 1),
                CancellationToken.None));

        Assert.Equal("invalid_date_range", exception.Code);
    }

    [Fact]
    public async Task RegisterIncidentAsync_WithoutAuthenticatedUser_ShouldFail()
    {
        var repository = new FakeCollectionRepository();
        var collection = SeedCollection(repository);
        var useCases = CreateUseCases(repository, fixedNow: Now(), currentUser: new FakeCurrentUser(""));

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCases.RegisterIncidentAsync(collection.Id, "Delay on route.", CancellationToken.None));

        Assert.Equal("authenticated_user_required", exception.Code);
    }

    private static CollectionUseCases CreateUseCases(FakeCollectionRepository repository, DateTimeOffset fixedNow, ICurrentUser? currentUser = null)
    {
        return new CollectionUseCases(
            repository,
            new FakeCollectionNumberGenerator("COL-0001"),
            new FakeClock(fixedNow, new DateOnly(2026, 5, 28)),
            currentUser ?? new FakeCurrentUser("operator.test"),
            NullLogger<CollectionUseCases>.Instance);
    }

    private static Collection SeedCollection(FakeCollectionRepository repository)
    {
        var customerId = Guid.NewGuid();
        repository.Customers[customerId] = new Customer(customerId, "Test Customer");
        var collection = new Collection(
            Guid.NewGuid(),
            "COL-TEST",
            customerId,
            "Test Customer",
            "Sender",
            "Street A",
            "Recipient",
            "Street B",
            new DateOnly(2026, 5, 28),
            CollectionPriority.Normal,
            null,
            Now());
        repository.Collections[collection.Id] = collection;
        return collection;
    }

    private static DateTimeOffset Now() => new(2026, 5, 28, 12, 0, 0, TimeSpan.Zero);

    private sealed class FakeCollectionRepository : ICollectionRepository
    {
        public Dictionary<Guid, Collection> Collections { get; } = [];
        public Dictionary<Guid, Customer> Customers { get; } = [];
        public Dictionary<Guid, Driver> Drivers { get; } = [];
        public Dictionary<Guid, Vehicle> Vehicles { get; } = [];

        public Task AddAsync(Collection collection, CancellationToken cancellationToken)
        {
            Collections[collection.Id] = collection;
            return Task.CompletedTask;
        }

        public Task<Collection?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(Collections.GetValueOrDefault(id));

        public Task<IReadOnlyList<Collection>> ListAsync(
            CollectionStatus? status,
            Guid? customerId,
            DateOnly? startDate,
            DateOnly? endDate,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Collection>>(Collections.Values.ToList());

        public Task<Customer?> GetCustomerAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(Customers.GetValueOrDefault(id));

        public Task<Driver?> GetDriverAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(Drivers.GetValueOrDefault(id));

        public Task<Vehicle?> GetVehicleAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(Vehicles.GetValueOrDefault(id));

        public Task<IReadOnlyList<OptionDto>> ListCustomersAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<OptionDto>>(Customers.Values.Select(c => new OptionDto(c.Id, c.Name)).ToList());

        public Task<IReadOnlyList<OptionDto>> ListDriversAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<OptionDto>>(Drivers.Values.Select(d => new OptionDto(d.Id, d.Name)).ToList());

        public Task<IReadOnlyList<VehicleOptionDto>> ListVehiclesAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<VehicleOptionDto>>(
                Vehicles.Values.Select(v => new VehicleOptionDto(v.Id, v.Plate, v.Description)).ToList());

        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class FakeClock(DateTimeOffset now, DateOnly today) : IClock
    {
        public DateTimeOffset Now => now;
        public DateOnly Today => today;
    }

    private sealed class FakeCollectionNumberGenerator(string number) : ICollectionNumberGenerator
    {
        public string Generate() => number;
    }

    private sealed class FakeCurrentUser : ICurrentUser
    {
        public FakeCurrentUser(string? displayName = "operator.test")
        {
            DisplayName = displayName;
            IsAuthenticated = !string.IsNullOrWhiteSpace(displayName);
            UserId = IsAuthenticated ? Guid.NewGuid() : null;
        }

        public Guid? UserId { get; }
        public string? Email => null;
        public string? DisplayName { get; }
        public bool IsAuthenticated { get; }
    }
}
