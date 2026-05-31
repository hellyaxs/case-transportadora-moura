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
    public async Task CreateCollectionUseCase_ShouldPersistOpenCollectionWithAssignment()
    {
        var repository = new FakeCollectionRepository();
        var customerId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        repository.Customers[customerId] = new Customer(customerId, "Test Customer");
        repository.Drivers[driverId] = new Driver(driverId, "Ana Souza");
        repository.Vehicles[vehicleId] = new Vehicle(vehicleId, "ABC1D23", "Van");
        var useCase = CreateCreateUseCase(repository, fixedNow: Now());

        var result = await useCase.ExecuteAsync(
            new CreateCollectionDto(
                customerId,
                "Sender",
                "Street A, 1",
                "Recipient",
                "Street B, 2",
                new DateOnly(2026, 5, 28),
                CollectionPriority.High,
                "Notes",
                driverId,
                vehicleId),
            CancellationToken.None);

        Assert.Equal(CollectionStatus.Open, result.Status);
        Assert.Equal("COL-0001", result.Number);
        Assert.Equal("Ana Souza", result.DriverName);
        Assert.Equal("ABC1D23", result.VehiclePlate);
        Assert.NotNull(result.AssignedAt);
        Assert.Single(repository.Collections);
    }

    [Fact]
    public async Task CreateCollectionUseCase_MissingCustomer_ShouldFail()
    {
        var useCase = CreateCreateUseCase(new FakeCollectionRepository(), fixedNow: Now());

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCase.ExecuteAsync(
                new CreateCollectionDto(
                    Guid.NewGuid(),
                    "Sender",
                    "Street A, 1",
                    "Recipient",
                    "Street B, 2",
                    new DateOnly(2026, 5, 28),
                    null,
                    null,
                    Guid.NewGuid(),
                    Guid.NewGuid()),
                CancellationToken.None));

        Assert.Equal("customer_not_found", exception.Code);
    }

    [Fact]
    public async Task CreateCollectionUseCase_MissingDriver_ShouldFail()
    {
        var repository = new FakeCollectionRepository();
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        repository.Customers[customerId] = new Customer(customerId, "Test Customer");
        repository.Vehicles[vehicleId] = new Vehicle(vehicleId, "ABC1D23", "Van");
        var useCase = CreateCreateUseCase(repository, fixedNow: Now());

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCase.ExecuteAsync(
                new CreateCollectionDto(
                    customerId,
                    "Sender",
                    "Street A, 1",
                    "Recipient",
                    "Street B, 2",
                    new DateOnly(2026, 5, 28),
                    null,
                    null,
                    Guid.NewGuid(),
                    vehicleId),
                CancellationToken.None));

        Assert.Equal("driver_not_found", exception.Code);
    }

    [Fact]
    public async Task ListCollectionsUseCase_ShouldReturnPaginatedResponseWithMetrics()
    {
        var repository = new FakeCollectionRepository();
        var customerId = Guid.NewGuid();
        repository.Customers[customerId] = new Customer(customerId, "Test Customer");
        repository.Collections[Guid.NewGuid()] = CreateSeedCollection(customerId, CollectionStatus.Open, CollectionPriority.High);
        repository.Collections[Guid.NewGuid()] = CreateSeedCollection(customerId, CollectionStatus.InProgress, CollectionPriority.Normal);
        var useCase = CreateListUseCase(repository, fixedNow: Now());

        var result = await useCase.ExecuteAsync(null, null, null, null, 1, 1, CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(1, result.Metrics.OpenCount);
        Assert.Equal(1, result.Metrics.InProgressCount);
        Assert.Equal(1, result.Metrics.HighPriorityCount);
    }

    [Fact]
    public async Task RegisterCollectionIncidentUseCase_ShouldPersistResponsibleUser()
    {
        var repository = new FakeCollectionRepository();
        var collection = SeedCollection(repository);
        var useCase = CreateRegisterIncidentUseCase(repository, fixedNow: Now(), currentUser: new FakeCurrentUser("operator.test"));

        var result = await useCase.ExecuteAsync(
            collection.Id,
            "Sender absent.",
            CancellationToken.None);

        var incident = Assert.Single(result.Incidents);
        Assert.Equal("operator.test", incident.ResponsibleUser);
        Assert.Equal("Sender absent.", incident.Description);
    }

    [Fact]
    public async Task ListCollectionsUseCase_InvalidDateRange_ShouldFail()
    {
        var useCase = CreateListUseCase(new FakeCollectionRepository(), fixedNow: Now());

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCase.ExecuteAsync(
                null,
                null,
                new DateOnly(2026, 5, 10),
                new DateOnly(2026, 5, 1),
                1,
                10,
                CancellationToken.None));

        Assert.Equal("invalid_date_range", exception.Code);
    }

    [Fact]
    public async Task RegisterCollectionIncidentUseCase_WithoutAuthenticatedUser_ShouldFail()
    {
        var repository = new FakeCollectionRepository();
        var collection = SeedCollection(repository);
        var useCase = CreateRegisterIncidentUseCase(repository, fixedNow: Now(), currentUser: new FakeCurrentUser(""));

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCase.ExecuteAsync(collection.Id, "Delay on route.", CancellationToken.None));

        Assert.Equal("authenticated_user_required", exception.Code);
    }

    [Fact]
    public async Task DeleteCollectionUseCase_CollectedCollection_ShouldRemoveRecord()
    {
        var repository = new FakeCollectionRepository();
        var customerId = Guid.NewGuid();
        repository.Customers[customerId] = new Customer(customerId, "Test Customer");
        var collection = CreateSeedCollection(customerId, CollectionStatus.Collected, CollectionPriority.Normal);
        repository.Collections[collection.Id] = collection;
        var useCase = CreateDeleteUseCase(repository);

        await useCase.ExecuteAsync(collection.Id, CancellationToken.None);

        Assert.Empty(repository.Collections);
    }

    [Fact]
    public async Task DeleteCollectionUseCase_OpenCollection_ShouldFail()
    {
        var repository = new FakeCollectionRepository();
        var collection = SeedCollection(repository);
        var useCase = CreateDeleteUseCase(repository);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCase.ExecuteAsync(collection.Id, CancellationToken.None));

        Assert.Equal("collection_not_deletable", exception.Code);
        Assert.Single(repository.Collections);
    }

    [Fact]
    public async Task DeleteCollectionUseCase_MissingCollection_ShouldFail()
    {
        var useCase = CreateDeleteUseCase(new FakeCollectionRepository());

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), CancellationToken.None));
    }

    private static CreateCollectionUseCase CreateCreateUseCase(FakeCollectionRepository repository, DateTimeOffset fixedNow)
    {
        return new CreateCollectionUseCase(
            repository,
            new FakeCollectionNumberGenerator("COL-0001"),
            CreateClock(fixedNow),
            NullLogger<CreateCollectionUseCase>.Instance);
    }

    private static ListCollectionsUseCase CreateListUseCase(FakeCollectionRepository repository, DateTimeOffset fixedNow)
    {
        return new ListCollectionsUseCase(repository, CreateClock(fixedNow));
    }

    private static RegisterCollectionIncidentUseCase CreateRegisterIncidentUseCase(
        FakeCollectionRepository repository,
        DateTimeOffset fixedNow,
        ICurrentUser currentUser)
    {
        return new RegisterCollectionIncidentUseCase(
            repository,
            CreateClock(fixedNow),
            currentUser,
            NullLogger<RegisterCollectionIncidentUseCase>.Instance);
    }

    private static DeleteCollectionUseCase CreateDeleteUseCase(FakeCollectionRepository repository)
    {
        return new DeleteCollectionUseCase(repository, NullLogger<DeleteCollectionUseCase>.Instance);
    }

    private static FakeClock CreateClock(DateTimeOffset fixedNow) => new(fixedNow, new DateOnly(2026, 5, 28));

    private static Collection SeedCollection(FakeCollectionRepository repository)
    {
        var customerId = Guid.NewGuid();
        repository.Customers[customerId] = new Customer(customerId, "Test Customer");
        var collection = CreateSeedCollection(customerId, CollectionStatus.Open, CollectionPriority.Normal);
        repository.Collections[collection.Id] = collection;
        return collection;
    }

    private static Collection CreateSeedCollection(Guid customerId, CollectionStatus status, CollectionPriority priority)
    {
        var collection = new Collection(
            Guid.NewGuid(),
            $"COL-{Guid.NewGuid():N}"[..12],
            customerId,
            "Test Customer",
            "Sender",
            "Street A",
            "Recipient",
            "Street B",
            new DateOnly(2026, 5, 28),
            priority,
            null,
            Now());

        if (status is CollectionStatus.InProgress or CollectionStatus.Collected)
        {
            collection.AssignDriverAndVehicle(Guid.NewGuid(), "Driver", Guid.NewGuid(), "ABC1D23", Now());
        }

        if (status == CollectionStatus.InProgress)
        {
            collection.MarkInProgress(Now());
        }

        if (status == CollectionStatus.Collected)
        {
            collection.MarkInProgress(Now());
            collection.MarkAsCollected(Now());
        }

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

        public Task<int> CountAsync(
            CollectionStatus? status,
            Guid? customerId,
            DateOnly? startDate,
            DateOnly? endDate,
            CancellationToken cancellationToken) =>
            Task.FromResult(ApplyFilters(status, customerId, startDate, endDate).Count);

        public Task<IReadOnlyList<Collection>> ListPagedAsync(
            CollectionStatus? status,
            Guid? customerId,
            DateOnly? startDate,
            DateOnly? endDate,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var items = ApplyFilters(status, customerId, startDate, endDate)
                .OrderByDescending(collection => collection.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult<IReadOnlyList<Collection>>(items);
        }

        public Task<CollectionListMetricsDto> GetListMetricsAsync(
            CollectionStatus? status,
            Guid? customerId,
            DateOnly? startDate,
            DateOnly? endDate,
            DateOnly today,
            CancellationToken cancellationToken)
        {
            var query = ApplyFilters(status, customerId, startDate, endDate);

            return Task.FromResult(new CollectionListMetricsDto(
                query.Count(collection => collection.Status == CollectionStatus.Open),
                query.Count(collection => collection.Status == CollectionStatus.InProgress),
                query.Count(collection => collection.IsOverdue(today)),
                query.Count(collection => collection.Priority == CollectionPriority.High)));
        }

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

        public Task DeleteAsync(Collection collection, CancellationToken cancellationToken)
        {
            Collections.Remove(collection.Id);
            return Task.CompletedTask;
        }

        private List<Collection> ApplyFilters(
            CollectionStatus? status,
            Guid? customerId,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            IEnumerable<Collection> query = Collections.Values;

            if (status.HasValue)
            {
                query = query.Where(collection => collection.Status == status);
            }

            if (customerId.HasValue)
            {
                query = query.Where(collection => collection.CustomerId == customerId);
            }

            if (startDate.HasValue)
            {
                query = query.Where(collection => collection.ExpectedPickupDate >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(collection => collection.ExpectedPickupDate <= endDate);
            }

            return query.ToList();
        }
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
