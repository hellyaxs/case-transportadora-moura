namespace Api.Modules.Collections.Domain.Entities;

public sealed class Driver
{
    private Driver()
    {
    }

    public Driver(Guid id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Driver name is required.", nameof(name));
        }

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Name = name.Trim();
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
}
