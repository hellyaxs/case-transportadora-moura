using Api.Modules.Collections.Application.Dtos;
using Api.Modules.Collections.Domain.Entities;
using Api.Modules.Collections.Domain.Enums;

namespace Api.Modules.Collections.Application.Contracts;

public interface ICollectionRepository
{
    Task AddAsync(Collection collection, CancellationToken cancellationToken);
    Task<Collection?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<int> CountAsync(CollectionStatus? status, Guid? customerId, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken);
    Task<IReadOnlyList<Collection>> ListPagedAsync(
        CollectionStatus? status,
        Guid? customerId,
        DateOnly? startDate,
        DateOnly? endDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
    Task<CollectionListMetricsDto> GetListMetricsAsync(
        CollectionStatus? status,
        Guid? customerId,
        DateOnly? startDate,
        DateOnly? endDate,
        DateOnly today,
        CancellationToken cancellationToken);
    Task<Customer?> GetCustomerAsync(Guid id, CancellationToken cancellationToken);
    Task<Driver?> GetDriverAsync(Guid id, CancellationToken cancellationToken);
    Task<Vehicle?> GetVehicleAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<OptionDto>> ListCustomersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<OptionDto>> ListDriversAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<VehicleOptionDto>> ListVehiclesAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task DeleteAsync(Collection collection, CancellationToken cancellationToken);
}
