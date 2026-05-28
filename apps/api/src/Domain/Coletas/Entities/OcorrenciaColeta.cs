using Api.Domain.Coletas.Exceptions;

namespace Api.Domain.Coletas.Entities;

public sealed class OcorrenciaColeta
{
    private OcorrenciaColeta()
    {
    }

    public OcorrenciaColeta(Guid id, Guid coletaId, string descricao, string usuarioResponsavel, DateTimeOffset registradaEm)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new BusinessRuleException("A ocorrência deve possuir uma descrição.", "ocorrencia_descricao_obrigatoria");
        }

        if (string.IsNullOrWhiteSpace(usuarioResponsavel))
        {
            throw new BusinessRuleException("A ocorrência deve possuir usuário responsável.", "ocorrencia_usuario_obrigatorio");
        }

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        ColetaId = coletaId;
        Descricao = descricao.Trim();
        UsuarioResponsavel = usuarioResponsavel.Trim();
        RegistradaEm = registradaEm;
    }

    public Guid Id { get; private set; }
    public Guid ColetaId { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public string UsuarioResponsavel { get; private set; } = string.Empty;
    public DateTimeOffset RegistradaEm { get; private set; }
}
