using Api.Application.Coletas.Contracts;
using Api.Application.Coletas.Dtos;
using Api.Domain.Coletas.Entities;
using Api.Domain.Coletas.Enums;
using Api.Infrastructure.Coletas.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Coletas.Repositories;

public sealed class EfColetaRepository(TransportadoraDbContext dbContext) : IColetaRepository
{
    public async Task AddAsync(Coleta coleta, CancellationToken cancellationToken)
    {
        await dbContext.Coletas.AddAsync(coleta, cancellationToken);
    }

    public async Task<Coleta?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Coletas
            .Include(coleta => coleta.Ocorrencias)
            .FirstOrDefaultAsync(coleta => coleta.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Coleta>> ListAsync(
        ColetaStatus? status,
        Guid? clienteId,
        DateOnly? dataInicial,
        DateOnly? dataFinal,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Coletas.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(coleta => coleta.Status == status);
        }

        if (clienteId.HasValue)
        {
            query = query.Where(coleta => coleta.ClienteId == clienteId);
        }

        if (dataInicial.HasValue)
        {
            query = query.Where(coleta => coleta.DataPrevistaRetirada >= dataInicial);
        }

        if (dataFinal.HasValue)
        {
            query = query.Where(coleta => coleta.DataPrevistaRetirada <= dataFinal);
        }

        return await query
            .OrderBy(coleta => coleta.DataPrevistaRetirada)
            .ThenBy(coleta => coleta.Prioridade == ColetaPrioridade.Alta ? 0 : 1)
            .ThenBy(coleta => coleta.Numero)
            .ToListAsync(cancellationToken);
    }

    public async Task<Cliente?> GetClienteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Clientes.FindAsync([id], cancellationToken);
    }

    public async Task<Motorista?> GetMotoristaAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Motoristas.FindAsync([id], cancellationToken);
    }

    public async Task<Veiculo?> GetVeiculoAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Veiculos.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<OptionDto>> ListClientesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Clientes
            .OrderBy(cliente => cliente.Nome)
            .Select(cliente => new OptionDto(cliente.Id, cliente.Nome))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OptionDto>> ListMotoristasAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Motoristas
            .OrderBy(motorista => motorista.Nome)
            .Select(motorista => new OptionDto(motorista.Id, motorista.Nome))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VeiculoOptionDto>> ListVeiculosAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Veiculos
            .OrderBy(veiculo => veiculo.Placa)
            .Select(veiculo => new VeiculoOptionDto(veiculo.Id, veiculo.Placa, veiculo.Descricao))
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
