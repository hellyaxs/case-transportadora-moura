using Api.Modules.Auth.Application.Contracts;
using Api.Modules.Auth.Application.UseCases;
using Api.Modules.Auth.Domain.Entities;
using Api.Modules.Auth.Infrastructure.Configuration;
using Api.Modules.Auth.Infrastructure.Security;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Api.Test.Modules.Auth.Application;

public class AuthUseCasesTests
{
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsUserAndToken()
    {
        var passwordHasher = new BcryptPasswordHasher();
        var user = new User(Guid.NewGuid(), "operator@test.local", "Operator Test", passwordHasher.Hash("secret123"));
        var repository = new FakeUserRepository(user);
        var useCases = CreateUseCases(repository, passwordHasher);

        var result = await useCases.LoginAsync("operator@test.local", "secret123", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Value.User.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.Token));
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        var passwordHasher = new BcryptPasswordHasher();
        var user = new User(Guid.NewGuid(), "operator@test.local", "Operator Test", passwordHasher.Hash("secret123"));
        var repository = new FakeUserRepository(user);
        var useCases = CreateUseCases(repository, passwordHasher);

        var result = await useCases.LoginAsync("operator@test.local", "wrong-password", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithUnknownEmail_ReturnsNull()
    {
        var useCases = CreateUseCases(new FakeUserRepository(null), new BcryptPasswordHasher());

        var result = await useCases.LoginAsync("missing@test.local", "secret123", CancellationToken.None);

        Assert.Null(result);
    }

    private static AuthUseCases CreateUseCases(FakeUserRepository repository, IPasswordHasher passwordHasher)
    {
        var jwtOptions = Options.Create(new JwtOptions
        {
            Secret = "test-secret-key-with-enough-length-for-hmac",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpirationHours = 1,
        });

        return new AuthUseCases(repository, passwordHasher, new JwtTokenService(jwtOptions), NullLogger<AuthUseCases>.Instance);
    }

    private sealed class FakeUserRepository(User? user) : IUserRepository
    {
        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
            Task.FromResult(user is not null && user.Email == email.Trim().ToLowerInvariant() ? user : null);
    }
}
