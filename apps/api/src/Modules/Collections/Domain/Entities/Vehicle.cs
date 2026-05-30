namespace Api.Modules.Collections.Domain.Entities;

public sealed class Vehicle
{
    private Vehicle()
    {
    }

    public Vehicle(Guid id, string plate, string description)
    {
        if (string.IsNullOrWhiteSpace(plate))
        {
            throw new ArgumentException("Vehicle plate is required.", nameof(plate));
        }

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Plate = plate.Trim().ToUpperInvariant();
        Description = description.Trim();
    }

    public Guid Id { get; private set; }
    public string Plate { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
}
