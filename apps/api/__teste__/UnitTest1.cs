using Api.Domain.Coletas.Entities;
using Api.Domain.Coletas.Enums;
using Api.Domain.Coletas.Exceptions;

namespace Api.Test;

public class ColetaDomainTests
{
    [Fact]
    public void CriarColeta_DeveIniciarAberta()
    {
        var coleta = CriarColeta();

        Assert.Equal(ColetaStatus.Aberta, coleta.Status);
        Assert.Equal(ColetaPrioridade.Normal, coleta.Prioridade);
        Assert.NotEqual(Guid.Empty, coleta.Id);
    }

    [Fact]
    public void ColetaAbertaComAtribuicao_DeveIrParaEmColetaEColetada()
    {
        var coleta = CriarColeta();

        coleta.AtribuirMotoristaVeiculo(Guid.NewGuid(), "Ana Souza", Guid.NewGuid(), "abc1d23", Agora());
        coleta.MarcarEmColeta(Agora().AddMinutes(10));
        coleta.MarcarComoColetada(Agora().AddMinutes(30));

        Assert.Equal(ColetaStatus.Coletada, coleta.Status);
        Assert.NotNull(coleta.AtribuidaEm);
        Assert.NotNull(coleta.IniciadaEm);
        Assert.NotNull(coleta.ColetadaEm);
    }

    [Fact]
    public void ColetaCancelada_NaoDeveVoltarParaEmColeta()
    {
        var coleta = CriarColeta();

        coleta.Cancelar("Remetente indisponível", Agora());

        var exception = Assert.Throws<BusinessRuleException>(() => coleta.MarcarEmColeta(Agora().AddMinutes(1)));
        Assert.Equal("coleta_cancelada_terminal", exception.Code);
    }

    [Fact]
    public void ColetaCancelada_NaoDeveVoltarParaColetada()
    {
        var coleta = CriarColeta();

        coleta.Cancelar("Endereço incorreto", Agora());

        var exception = Assert.Throws<BusinessRuleException>(() => coleta.MarcarComoColetada(Agora().AddMinutes(1)));
        Assert.Equal("coleta_cancelada_terminal", exception.Code);
    }

    [Fact]
    public void ColetaSemMotorista_NaoDeveSerColetada()
    {
        var coleta = CriarColeta();

        var exception = Assert.Throws<BusinessRuleException>(() => coleta.MarcarComoColetada(Agora()));
        Assert.Equal("motorista_obrigatorio", exception.Code);
    }

    [Fact]
    public void ColetaSemVeiculo_NaoDeveSerColetada()
    {
        var coleta = CriarColeta();
        SetPrivate(coleta, nameof(Coleta.MotoristaId), Guid.NewGuid());
        SetPrivate(coleta, nameof(Coleta.MotoristaNome), "Ana Souza");
        SetPrivate(coleta, nameof(Coleta.Status), ColetaStatus.EmColeta);

        var exception = Assert.Throws<BusinessRuleException>(() => coleta.MarcarComoColetada(Agora()));
        Assert.Equal("veiculo_obrigatorio", exception.Code);
    }

    [Fact]
    public void ColetaSemAtribuicao_NaoDeveIrParaEmColeta()
    {
        var coleta = CriarColeta();

        var exception = Assert.Throws<BusinessRuleException>(() => coleta.MarcarEmColeta(Agora()));
        Assert.Equal("motorista_obrigatorio", exception.Code);
    }

    [Fact]
    public void RegistrarOcorrencia_DeveGuardarDataHoraEUsuarioResponsavel()
    {
        var coleta = CriarColeta();
        var registradaEm = Agora();

        coleta.RegistrarOcorrencia("Remetente ausente no local.", "operador.teste", registradaEm);

        var ocorrencia = Assert.Single(coleta.Ocorrencias);
        Assert.Equal("Remetente ausente no local.", ocorrencia.Descricao);
        Assert.Equal("operador.teste", ocorrencia.UsuarioResponsavel);
        Assert.Equal(registradaEm, ocorrencia.RegistradaEm);
    }

    private static Coleta CriarColeta(ColetaPrioridade prioridade = ColetaPrioridade.Normal)
    {
        return new Coleta(
            Guid.NewGuid(),
            "COL-TESTE",
            Guid.NewGuid(),
            "Cliente Teste",
            "Remetente Teste",
            "Rua Origem, 100",
            "Destinatário Teste",
            "Rua Destino, 200",
            new DateOnly(2026, 5, 28),
            prioridade,
            "Observação",
            Agora());
    }

    private static DateTimeOffset Agora()
    {
        return new DateTimeOffset(2026, 5, 28, 12, 0, 0, TimeSpan.Zero);
    }

    private static void SetPrivate<T>(Coleta coleta, string propertyName, T value)
    {
        typeof(Coleta).GetProperty(propertyName)!.SetValue(coleta, value);
    }
}
