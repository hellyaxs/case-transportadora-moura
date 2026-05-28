namespace Api.Domain.Coletas.Entities;

public sealed class Cliente
{
    private Cliente()
    {
    }

    public Cliente(Guid id, string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("Nome do cliente é obrigatório.", nameof(nome));
        }

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Nome = nome.Trim();
    }

    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
}
