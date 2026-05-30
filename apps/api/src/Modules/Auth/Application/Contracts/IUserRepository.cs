using Api.Modules.Auth.Domain.Entities;

namespace Api.Modules.Auth.Application.Contracts;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
