namespace Api.Domain.Coletas.Entities;

public sealed class Veiculo
{
    private Veiculo()
    {
    }

    public Veiculo(Guid id, string placa, string descricao)
    {
        if (string.IsNullOrWhiteSpace(placa))
        {
            throw new ArgumentException("Placa do veículo é obrigatória.", nameof(placa));
        }

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Placa = placa.Trim().ToUpperInvariant();
        Descricao = descricao.Trim();
    }

    public Guid Id { get; private set; }
    public string Placa { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
}
