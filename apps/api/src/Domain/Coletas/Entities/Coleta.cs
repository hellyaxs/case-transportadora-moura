using Api.Domain.Coletas.Enums;
using Api.Domain.Coletas.Exceptions;

namespace Api.Domain.Coletas.Entities;

public sealed class Coleta
{
    private readonly List<OcorrenciaColeta> _ocorrencias = [];

    private Coleta()
    {
    }

    public Coleta(
        Guid id,
        string numero,
        Guid clienteId,
        string clienteNome,
        string remetenteNome,
        string remetenteEndereco,
        string destinatarioNome,
        string destinatarioEndereco,
        DateOnly dataPrevistaRetirada,
        ColetaPrioridade prioridade,
        string? observacoes,
        DateTimeOffset criadaEm)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Numero = Obrigatorio(numero, "Número da coleta é obrigatório.");
        ClienteId = clienteId == Guid.Empty ? throw new ArgumentException("Cliente é obrigatório.", nameof(clienteId)) : clienteId;
        ClienteNome = Obrigatorio(clienteNome, "Nome do cliente é obrigatório.");
        RemetenteNome = Obrigatorio(remetenteNome, "Remetente é obrigatório.");
        RemetenteEndereco = Obrigatorio(remetenteEndereco, "Endereço do remetente é obrigatório.");
        DestinatarioNome = Obrigatorio(destinatarioNome, "Destinatário é obrigatório.");
        DestinatarioEndereco = Obrigatorio(destinatarioEndereco, "Endereço do destinatário é obrigatório.");
        DataPrevistaRetirada = dataPrevistaRetirada;
        Prioridade = prioridade == default ? ColetaPrioridade.Normal : prioridade;
        Observacoes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes.Trim();
        Status = ColetaStatus.Aberta;
        CriadaEm = criadaEm;
    }

    public Guid Id { get; private set; }
    public string Numero { get; private set; } = string.Empty;
    public Guid ClienteId { get; private set; }
    public string ClienteNome { get; private set; } = string.Empty;
    public string RemetenteNome { get; private set; } = string.Empty;
    public string RemetenteEndereco { get; private set; } = string.Empty;
    public string DestinatarioNome { get; private set; } = string.Empty;
    public string DestinatarioEndereco { get; private set; } = string.Empty;
    public DateOnly DataPrevistaRetirada { get; private set; }
    public ColetaPrioridade Prioridade { get; private set; }
    public string? Observacoes { get; private set; }
    public ColetaStatus Status { get; private set; }
    public Guid? MotoristaId { get; private set; }
    public string? MotoristaNome { get; private set; }
    public Guid? VeiculoId { get; private set; }
    public string? VeiculoPlaca { get; private set; }
    public DateTimeOffset CriadaEm { get; private set; }
    public DateTimeOffset? AtribuidaEm { get; private set; }
    public DateTimeOffset? IniciadaEm { get; private set; }
    public DateTimeOffset? ColetadaEm { get; private set; }
    public DateTimeOffset? CanceladaEm { get; private set; }
    public string? MotivoCancelamento { get; private set; }
    public IReadOnlyCollection<OcorrenciaColeta> Ocorrencias => _ocorrencias.AsReadOnly();

    public bool EstaAtrasada(DateOnly hoje)
    {
        return (Status is ColetaStatus.Aberta or ColetaStatus.EmColeta) && DataPrevistaRetirada < hoje;
    }

    public void AtribuirMotoristaVeiculo(
        Guid motoristaId,
        string motoristaNome,
        Guid veiculoId,
        string veiculoPlaca,
        DateTimeOffset atribuidaEm)
    {
        GarantirFluxoAtivo("Não é possível atribuir motorista ou veículo a uma coleta cancelada.");

        if (motoristaId == Guid.Empty || string.IsNullOrWhiteSpace(motoristaNome))
        {
            throw new BusinessRuleException("Motorista é obrigatório para atribuir a coleta.", "motorista_obrigatorio");
        }

        if (veiculoId == Guid.Empty || string.IsNullOrWhiteSpace(veiculoPlaca))
        {
            throw new BusinessRuleException("Veículo é obrigatório para atribuir a coleta.", "veiculo_obrigatorio");
        }

        MotoristaId = motoristaId;
        MotoristaNome = motoristaNome.Trim();
        VeiculoId = veiculoId;
        VeiculoPlaca = veiculoPlaca.Trim().ToUpperInvariant();
        AtribuidaEm = atribuidaEm;
    }

    public void MarcarEmColeta(DateTimeOffset iniciadaEm)
    {
        GarantirFluxoAtivo("Coleta cancelada não pode voltar para Em Coleta.");
        GarantirAtribuicaoCompleta();

        if (Status != ColetaStatus.Aberta)
        {
            throw new BusinessRuleException("Somente coletas abertas podem iniciar execução.", "transicao_status_invalida");
        }

        Status = ColetaStatus.EmColeta;
        IniciadaEm = iniciadaEm;
    }

    public void MarcarComoColetada(DateTimeOffset coletadaEm)
    {
        GarantirFluxoAtivo("Coleta cancelada não pode voltar para Coletada.");
        GarantirAtribuicaoCompleta();

        if (Status != ColetaStatus.EmColeta)
        {
            throw new BusinessRuleException("Somente coletas em execução podem ser concluídas.", "transicao_status_invalida");
        }

        Status = ColetaStatus.Coletada;
        ColetadaEm = coletadaEm;
    }

    public void Cancelar(string motivo, DateTimeOffset canceladaEm)
    {
        if (Status == ColetaStatus.Cancelada)
        {
            throw new BusinessRuleException("Coleta já está cancelada.", "coleta_ja_cancelada");
        }

        if (Status == ColetaStatus.Coletada)
        {
            throw new BusinessRuleException("Coleta já coletada não pode ser cancelada.", "coleta_ja_coletada");
        }

        Status = ColetaStatus.Cancelada;
        MotivoCancelamento = string.IsNullOrWhiteSpace(motivo) ? "Cancelamento operacional" : motivo.Trim();
        CanceladaEm = canceladaEm;
    }

    public void RegistrarOcorrencia(string descricao, string usuarioResponsavel, DateTimeOffset registradaEm)
    {
        _ocorrencias.Add(new OcorrenciaColeta(Guid.NewGuid(), Id, descricao, usuarioResponsavel, registradaEm));
    }

    private void GarantirFluxoAtivo(string mensagem)
    {
        if (Status == ColetaStatus.Cancelada)
        {
            throw new BusinessRuleException(mensagem, "coleta_cancelada_terminal");
        }

        if (Status == ColetaStatus.Coletada)
        {
            throw new BusinessRuleException("Coleta coletada não pode voltar ao fluxo ativo.", "coleta_coletada_terminal");
        }
    }

    private void GarantirAtribuicaoCompleta()
    {
        if (MotoristaId is null)
        {
            throw new BusinessRuleException("Não é permitido avançar sem motorista vinculado.", "motorista_obrigatorio");
        }

        if (VeiculoId is null)
        {
            throw new BusinessRuleException("Não é permitido avançar sem veículo vinculado.", "veiculo_obrigatorio");
        }
    }

    private static string Obrigatorio(string valor, string mensagem)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new BusinessRuleException(mensagem, "campo_obrigatorio");
        }

        return valor.Trim();
    }
}
