namespace Api.Modules.Auth.Domain.Entities;

public sealed class User
{
    private User()
    {
    }

    public User(Guid id, string email, string name, string passwordHash)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("User id is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        Id = id;
        Email = email.Trim().ToLowerInvariant();
        Name = name.Trim();
        PasswordHash = passwordHash;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
}
