using System.Text;
using Api.Modules.Auth.Application.Contracts;
using Api.Modules.Auth.Application.UseCases;
using Api.Modules.Auth.Infrastructure.Configuration;
using Api.Modules.Auth.Infrastructure.Persistence;
using Api.Modules.Auth.Infrastructure.Repositories;
using Api.Modules.Auth.Infrastructure.Security;
using Api.Modules.Auth.Presentation;
using Api.Modules.Auth.Presentation.Validators;
using Api.Shared.Application.Contracts;
using Api.Shared.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Api.Modules.Auth;

public static class AuthModule
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(options =>
        {
            configuration.GetSection(JwtOptions.SectionName).Bind(options);
            options.Secret = configuration["JWT_SECRET"]
                ?? configuration["Jwt:Secret"]
                ?? options.Secret;
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<AuthUseCases>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        var jwtOptions = new JwtOptions
        {
            Secret = configuration["JWT_SECRET"] ?? configuration["Jwt:Secret"] ?? string.Empty,
            Issuer = configuration["Jwt:Issuer"] ?? "transportadora-moura-api",
            Audience = configuration["Jwt:Audience"] ?? "transportadora-moura-web",
        };

        if (string.IsNullOrWhiteSpace(jwtOptions.Secret))
        {
            throw new InvalidOperationException("JWT_SECRET must be configured for authentication.");
        }

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ClockSkew = TimeSpan.FromMinutes(1),
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var cookieName = configuration["Jwt:CookieName"] ?? "auth_token";
                        if (context.Request.Cookies.TryGetValue(cookieName, out var token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    },
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static WebApplication MapAuthModule(this WebApplication app)
    {
        app.MapAuthEndpoints();
        return app;
    }

    public static void SeedAuthData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TransportadoraDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        AuthSeedData.Seed(dbContext, passwordHasher);
    }
}
