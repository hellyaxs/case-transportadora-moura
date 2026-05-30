using Api.Modules.Auth.Application.Contracts;
using Api.Modules.Auth.Domain.Entities;
using Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Modules.Auth.Infrastructure.Repositories;

public sealed class EfUserRepository(TransportadoraDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }
}
