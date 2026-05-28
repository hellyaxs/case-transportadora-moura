using Api.Application.Coletas.Dtos;
using Api.Domain.Coletas.Entities;
using Api.Domain.Coletas.Enums;

namespace Api.Application.Coletas.Contracts;

public interface IColetaRepository
{
    Task AddAsync(Coleta coleta, CancellationToken cancellationToken);
    Task<Coleta?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Coleta>> ListAsync(ColetaStatus? status, Guid? clienteId, DateOnly? dataInicial, DateOnly? dataFinal, CancellationToken cancellationToken);
    Task<Cliente?> GetClienteAsync(Guid id, CancellationToken cancellationToken);
    Task<Motorista?> GetMotoristaAsync(Guid id, CancellationToken cancellationToken);
    Task<Veiculo?> GetVeiculoAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<OptionDto>> ListClientesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<OptionDto>> ListMotoristasAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<VeiculoOptionDto>> ListVeiculosAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
