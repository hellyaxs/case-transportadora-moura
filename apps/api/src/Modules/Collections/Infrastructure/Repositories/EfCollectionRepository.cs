using Api.Modules.Collections.Application.Contracts;
using Api.Modules.Collections.Application.Dtos;
using Api.Modules.Collections.Domain.Entities;
using Api.Modules.Collections.Domain.Enums;
using Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Modules.Collections.Infrastructure.Repositories;

public sealed class EfCollectionRepository(TransportadoraDbContext dbContext) : ICollectionRepository
{
    public async Task AddAsync(Collection collection, CancellationToken cancellationToken)
    {
        await dbContext.Collections.AddAsync(collection, cancellationToken);
    }

    public async Task<Collection?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Collections
            .Include(collection => collection.Incidents)
            .FirstOrDefaultAsync(collection => collection.Id == id, cancellationToken);
    }

    public Task<int> CountAsync(
        CollectionStatus? status,
        Guid? customerId,
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken cancellationToken)
    {
        return ApplyFilters(status, customerId, startDate, endDate).CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Collection>> ListPagedAsync(
        CollectionStatus? status,
        Guid? customerId,
        DateOnly? startDate,
        DateOnly? endDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return await ApplyFilters(status, customerId, startDate, endDate)
            .OrderByDescending(collection => collection.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<CollectionListMetricsDto> GetListMetricsAsync(
        CollectionStatus? status,
        Guid? customerId,
        DateOnly? startDate,
        DateOnly? endDate,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        var query = ApplyFilters(status, customerId, startDate, endDate);

        return new CollectionListMetricsDto(
            await query.CountAsync(collection => collection.Status == CollectionStatus.Open, cancellationToken),
            await query.CountAsync(collection => collection.Status == CollectionStatus.InProgress, cancellationToken),
            await query.CountAsync(
                collection =>
                    (collection.Status == CollectionStatus.Open || collection.Status == CollectionStatus.InProgress)
                    && collection.ExpectedPickupDate < today,
                cancellationToken),
            await query.CountAsync(collection => collection.Priority == CollectionPriority.High, cancellationToken));
    }

    public async Task<Customer?> GetCustomerAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Customers.FindAsync([id], cancellationToken);
    }

    public async Task<Driver?> GetDriverAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Drivers.FindAsync([id], cancellationToken);
    }

    public async Task<Vehicle?> GetVehicleAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Vehicles.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<OptionDto>> ListCustomersAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Customers
            .OrderBy(customer => customer.Name)
            .Select(customer => new OptionDto(customer.Id, customer.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OptionDto>> ListDriversAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Drivers
            .OrderBy(driver => driver.Name)
            .Select(driver => new OptionDto(driver.Id, driver.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VehicleOptionDto>> ListVehiclesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Vehicles
            .OrderBy(vehicle => vehicle.Plate)
            .Select(vehicle => new VehicleOptionDto(vehicle.Id, vehicle.Plate, vehicle.Description))
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(Collection collection, CancellationToken cancellationToken)
    {
        dbContext.Collections.Remove(collection);
        return Task.CompletedTask;
    }

    private IQueryable<Collection> ApplyFilters(
        CollectionStatus? status,
        Guid? customerId,
        DateOnly? startDate,
        DateOnly? endDate)
    {
        var query = dbContext.Collections.AsQueryable();

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

        return query;
    }
}
