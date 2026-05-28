using Api.Application.Coletas.Dtos;
using Api.Domain.Coletas.Enums;

namespace Api.Presentation.Coletas;

public sealed record CriarColetaRequest(
    Guid ClienteId,
    string RemetenteNome,
    string RemetenteEndereco,
    string DestinatarioNome,
    string DestinatarioEndereco,
    DateOnly DataPrevistaRetirada,
    ColetaPrioridade? Prioridade,
    string? Observacoes)
{
    public CriarColetaDto ToDto()
    {
        return new CriarColetaDto(
            ClienteId,
            RemetenteNome,
            RemetenteEndereco,
            DestinatarioNome,
            DestinatarioEndereco,
            DataPrevistaRetirada,
            Prioridade,
            Observacoes);
    }
}

public sealed record AtribuirColetaRequest(Guid MotoristaId, Guid VeiculoId);

public sealed record CancelarColetaRequest(string? Motivo);

public sealed record RegistrarOcorrenciaRequest(string Descricao, string UsuarioResponsavel);
