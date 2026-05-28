using Api.Application.Coletas.Contracts;
using Api.Application.Coletas.Dtos;
using Api.Domain.Coletas.Entities;
using Api.Domain.Coletas.Enums;
using Api.Domain.Coletas.Exceptions;

namespace Api.Application.Coletas.UseCases;

public sealed class ColetaUseCases(
    IColetaRepository repository,
    IColetaNumeroGenerator numeroGenerator,
    IClock clock)
{
    public async Task<ColetaDetalheDto> CriarAsync(CriarColetaDto input, CancellationToken cancellationToken)
    {
        var cliente = await repository.GetClienteAsync(input.ClienteId, cancellationToken)
            ?? throw new BusinessRuleException("Cliente informado não foi encontrado.", "cliente_nao_encontrado");

        var coleta = new Coleta(
            Guid.NewGuid(),
            numeroGenerator.Gerar(),
            cliente.Id,
            cliente.Nome,
            input.RemetenteNome,
            input.RemetenteEndereco,
            input.DestinatarioNome,
            input.DestinatarioEndereco,
            input.DataPrevistaRetirada,
            input.Prioridade ?? ColetaPrioridade.Normal,
            input.Observacoes,
            clock.Now);

        await repository.AddAsync(coleta, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDetalhe(coleta, clock.Today);
    }

    public async Task<IReadOnlyList<ColetaResumoDto>> ListarAsync(
        ColetaStatus? status,
        Guid? clienteId,
        DateOnly? dataInicial,
        DateOnly? dataFinal,
        CancellationToken cancellationToken)
    {
        if (dataInicial.HasValue && dataFinal.HasValue && dataInicial > dataFinal)
        {
            throw new BusinessRuleException("Data inicial não pode ser maior que a data final.", "periodo_invalido");
        }

        var coletas = await repository.ListAsync(status, clienteId, dataInicial, dataFinal, cancellationToken);
        return coletas.Select(coleta => ToResumo(coleta, clock.Today)).ToList();
    }

    public async Task<ColetaDetalheDto> ObterDetalheAsync(Guid id, CancellationToken cancellationToken)
    {
        var coleta = await ObterColetaAsync(id, cancellationToken);
        return ToDetalhe(coleta, clock.Today);
    }

    public async Task<ColetaDetalheDto> AtribuirAsync(Guid id, Guid motoristaId, Guid veiculoId, CancellationToken cancellationToken)
    {
        var coleta = await ObterColetaAsync(id, cancellationToken);
        var motorista = await repository.GetMotoristaAsync(motoristaId, cancellationToken)
            ?? throw new BusinessRuleException("Motorista informado não foi encontrado.", "motorista_nao_encontrado");
        var veiculo = await repository.GetVeiculoAsync(veiculoId, cancellationToken)
            ?? throw new BusinessRuleException("Veículo informado não foi encontrado.", "veiculo_nao_encontrado");

        coleta.AtribuirMotoristaVeiculo(motorista.Id, motorista.Nome, veiculo.Id, veiculo.Placa, clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDetalhe(coleta, clock.Today);
    }

    public async Task<ColetaDetalheDto> MarcarEmColetaAsync(Guid id, CancellationToken cancellationToken)
    {
        var coleta = await ObterColetaAsync(id, cancellationToken);
        coleta.MarcarEmColeta(clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDetalhe(coleta, clock.Today);
    }

    public async Task<ColetaDetalheDto> ConcluirAsync(Guid id, CancellationToken cancellationToken)
    {
        var coleta = await ObterColetaAsync(id, cancellationToken);
        coleta.MarcarComoColetada(clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDetalhe(coleta, clock.Today);
    }

    public async Task<ColetaDetalheDto> CancelarAsync(Guid id, string? motivo, CancellationToken cancellationToken)
    {
        var coleta = await ObterColetaAsync(id, cancellationToken);
        coleta.Cancelar(motivo ?? "Cancelamento operacional", clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDetalhe(coleta, clock.Today);
    }

    public async Task<ColetaDetalheDto> RegistrarOcorrenciaAsync(
        Guid id,
        string descricao,
        string usuarioResponsavel,
        CancellationToken cancellationToken)
    {
        var coleta = await ObterColetaAsync(id, cancellationToken);
        coleta.RegistrarOcorrencia(descricao, usuarioResponsavel, clock.Now);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDetalhe(coleta, clock.Today);
    }

    private async Task<Coleta> ObterColetaAsync(Guid id, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Coleta não encontrada.");
    }

    private static ColetaResumoDto ToResumo(Coleta coleta, DateOnly hoje)
    {
        return new ColetaResumoDto(
            coleta.Id,
            coleta.Numero,
            coleta.ClienteId,
            coleta.ClienteNome,
            coleta.RemetenteNome,
            coleta.DestinatarioNome,
            coleta.DataPrevistaRetirada,
            coleta.Prioridade,
            coleta.Status,
            coleta.MotoristaNome,
            coleta.VeiculoPlaca,
            coleta.EstaAtrasada(hoje));
    }

    private static ColetaDetalheDto ToDetalhe(Coleta coleta, DateOnly hoje)
    {
        return new ColetaDetalheDto(
            coleta.Id,
            coleta.Numero,
            coleta.ClienteId,
            coleta.ClienteNome,
            coleta.RemetenteNome,
            coleta.RemetenteEndereco,
            coleta.DestinatarioNome,
            coleta.DestinatarioEndereco,
            coleta.DataPrevistaRetirada,
            coleta.Prioridade,
            coleta.Observacoes,
            coleta.Status,
            coleta.MotoristaId,
            coleta.MotoristaNome,
            coleta.VeiculoId,
            coleta.VeiculoPlaca,
            coleta.CriadaEm,
            coleta.AtribuidaEm,
            coleta.IniciadaEm,
            coleta.ColetadaEm,
            coleta.CanceladaEm,
            coleta.MotivoCancelamento,
            coleta.EstaAtrasada(hoje),
            coleta.Ocorrencias
                .OrderBy(ocorrencia => ocorrencia.RegistradaEm)
                .Select(ocorrencia => new OcorrenciaDto(
                    ocorrencia.Id,
                    ocorrencia.Descricao,
                    ocorrencia.UsuarioResponsavel,
                    ocorrencia.RegistradaEm))
                .ToList());
    }
}
