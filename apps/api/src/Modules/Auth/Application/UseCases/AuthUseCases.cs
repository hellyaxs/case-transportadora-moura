using Api.Modules.Auth.Application.Contracts;
using Api.Modules.Auth.Application.Dtos;
using Microsoft.Extensions.Logging;

namespace Api.Modules.Auth.Application.UseCases;

public sealed class AuthUseCases(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    ILogger<AuthUseCases> logger)
{
    public async Task<(AuthenticatedUserDto User, string Token)?> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null || !passwordHasher.Verify(password, user.PasswordHash))
        {
            logger.LogWarning("Login failed for email {Email}", email);
            return null;
        }

        logger.LogInformation("User {UserId} ({Email}) logged in successfully", user.Id, user.Email);
        var token = jwtTokenService.CreateToken(user);
        return (new AuthenticatedUserDto(user.Id, user.Email, user.Name), token);
    }
}
