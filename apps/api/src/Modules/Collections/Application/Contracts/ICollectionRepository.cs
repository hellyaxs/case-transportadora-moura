using Api.Modules.Collections.Application.Dtos;
using Api.Modules.Collections.Domain.Entities;
using Api.Modules.Collections.Domain.Enums;

namespace Api.Modules.Collections.Application.Contracts;

public interface ICollectionRepository
{
    Task AddAsync(Collection collection, CancellationToken cancellationToken);
    Task<Collection?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Collection>> ListAsync(CollectionStatus? status, Guid? customerId, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken);
    Task<Customer?> GetCustomerAsync(Guid id, CancellationToken cancellationToken);
    Task<Driver?> GetDriverAsync(Guid id, CancellationToken cancellationToken);
    Task<Vehicle?> GetVehicleAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<OptionDto>> ListCustomersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<OptionDto>> ListDriversAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<VehicleOptionDto>> ListVehiclesAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
