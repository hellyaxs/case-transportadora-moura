using Api.Domain.Coletas.Enums;

namespace Api.Application.Coletas.Dtos;

public sealed record CriarColetaDto(
    Guid ClienteId,
    string RemetenteNome,
    string RemetenteEndereco,
    string DestinatarioNome,
    string DestinatarioEndereco,
    DateOnly DataPrevistaRetirada,
    ColetaPrioridade? Prioridade,
    string? Observacoes);

public sealed record ColetaResumoDto(
    Guid Id,
    string Numero,
    Guid ClienteId,
    string ClienteNome,
    string RemetenteNome,
    string DestinatarioNome,
    DateOnly DataPrevistaRetirada,
    ColetaPrioridade Prioridade,
    ColetaStatus Status,
    string? MotoristaNome,
    string? VeiculoPlaca,
    bool Atrasada);

public sealed record ColetaDetalheDto(
    Guid Id,
    string Numero,
    Guid ClienteId,
    string ClienteNome,
    string RemetenteNome,
    string RemetenteEndereco,
    string DestinatarioNome,
    string DestinatarioEndereco,
    DateOnly DataPrevistaRetirada,
    ColetaPrioridade Prioridade,
    string? Observacoes,
    ColetaStatus Status,
    Guid? MotoristaId,
    string? MotoristaNome,
    Guid? VeiculoId,
    string? VeiculoPlaca,
    DateTimeOffset CriadaEm,
    DateTimeOffset? AtribuidaEm,
    DateTimeOffset? IniciadaEm,
    DateTimeOffset? ColetadaEm,
    DateTimeOffset? CanceladaEm,
    string? MotivoCancelamento,
    bool Atrasada,
    IReadOnlyList<OcorrenciaDto> Ocorrencias);

public sealed record OcorrenciaDto(
    Guid Id,
    string Descricao,
    string UsuarioResponsavel,
    DateTimeOffset RegistradaEm);

public sealed record OptionDto(Guid Id, string Nome);

public sealed record VeiculoOptionDto(Guid Id, string Placa, string Descricao);
