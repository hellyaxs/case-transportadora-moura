using Api.Modules.Auth.Application.Contracts;
using Api.Modules.Auth.Domain.Entities;
using Api.Shared.Infrastructure.Persistence;

namespace Api.Modules.Auth.Infrastructure.Persistence;

public static class AuthSeedData
{
    public const string DemoEmail = "operador@moura.local";
    public const string DemoPassword = "Moura@2026";
    public const string DemoName = "Operador Demo";

    public static void Seed(TransportadoraDbContext dbContext, IPasswordHasher passwordHasher)
    {
        if (dbContext.Users.Any())
        {
            return;
        }

        dbContext.Users.Add(new User(
            AuthSeedIds.DemoOperatorId,
            DemoEmail,
            DemoName,
            passwordHasher.Hash(DemoPassword)));
        dbContext.SaveChanges();
    }
}
