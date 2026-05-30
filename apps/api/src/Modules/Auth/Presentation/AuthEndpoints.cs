using Api.Modules.Auth.Application.Dtos;
using Api.Modules.Auth.Application.UseCases;
using Api.Modules.Auth.Infrastructure.Configuration;
using Api.Modules.Auth.Presentation.Validators;
using Api.Shared.Application.Contracts;
using Api.Shared.Presentation;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Api.Modules.Auth.Presentation;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .WithSummary("Authenticate an operational user")
            .WithDescription("Validates credentials and stores the signed JWT in an HttpOnly cookie.")
            .Produces<AuthenticatedUserDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", LogoutAsync)
            .WithName("Logout")
            .WithSummary("End the current session")
            .WithDescription("Removes the authentication cookie from the client.")
            .Produces(StatusCodes.Status204NoContent);

        group.MapGet("/me", GetCurrentUserAsync)
            .WithName("GetCurrentUser")
            .WithSummary("Get the authenticated user")
            .WithDescription("Returns the current session user when the HttpOnly cookie is valid.")
            .Produces<AuthenticatedUserDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return group;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        IValidator<LoginRequest> validator,
        AuthUseCases authUseCases,
        IOptions<JwtOptions> jwtOptions,
        CancellationToken cancellationToken)
    {
        var validationError = await EndpointValidation.ValidateAsync(request, validator, cancellationToken);
        if (validationError is not null)
        {
            return validationError;
        }

        var result = await authUseCases.LoginAsync(request.Email, request.Password, cancellationToken);
        if (result is null)
        {
            return Results.Problem(
                title: "Authentication failed",
                detail: "Invalid email or password.",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var options = jwtOptions.Value;
        return Results.Ok(result.Value.User)
            .WithCookie(options.CookieName, result.Value.Token, BuildCookieOptions(options));
    }

    private static IResult LogoutAsync(IOptions<JwtOptions> jwtOptions)
    {
        var options = jwtOptions.Value;
        return Results.NoContent()
            .WithCookie(options.CookieName, string.Empty, BuildCookieOptions(options, expired: true));
    }

    private static IResult GetCurrentUserAsync(ICurrentUser currentUser)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
        {
            return Results.Problem(
                title: "Unauthorized",
                detail: "Authentication is required.",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return Results.Ok(new AuthenticatedUserDto(
            currentUser.UserId.Value,
            currentUser.Email ?? string.Empty,
            currentUser.DisplayName ?? string.Empty));
    }

    private static CookieOptions BuildCookieOptions(JwtOptions options, bool expired = false)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = expired
                ? DateTimeOffset.UtcNow.AddDays(-1)
                : DateTimeOffset.UtcNow.AddHours(options.ExpirationHours),
        };
    }
}
