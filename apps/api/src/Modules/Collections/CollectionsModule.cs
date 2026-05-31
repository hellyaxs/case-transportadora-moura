using Api.Modules.Collections.Application.Contracts;
using Api.Modules.Collections.Application.UseCases;
using Api.Modules.Collections.Infrastructure;
using Api.Modules.Collections.Infrastructure.Repositories;
using Api.Modules.Collections.Presentation;
using Api.Modules.Collections.Presentation.Validators;
using Api.Shared.Infrastructure.Configuration;
using Api.Shared.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Modules.Collections;

public static class CollectionsModule
{
    public static IServiceCollection AddCollectionsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TransportadoraDbContext>(options =>
            options.UseNpgsql(PostgresConnectionStringFactory.Create(configuration)));
        services.AddScoped<ICollectionRepository, EfCollectionRepository>();
        services.AddScoped<CreateCollectionUseCase>();
        services.AddScoped<ListCollectionsUseCase>();
        services.AddScoped<GetCollectionDetailsUseCase>();
        services.AddScoped<StartCollectionUseCase>();
        services.AddScoped<CompleteCollectionUseCase>();
        services.AddScoped<CancelCollectionUseCase>();
        services.AddScoped<RegisterCollectionIncidentUseCase>();
        services.AddScoped<DeleteCollectionUseCase>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<ICollectionNumberGenerator, CollectionNumberGenerator>();
        services.AddValidatorsFromAssemblyContaining<RegisterIncidentRequestValidator>();

        return services;
    }

    public static WebApplication MapCollectionsModule(this WebApplication app)
    {
        app.MapCollectionsEndpoints();
        app.MapOperationalCatalogEndpoints();

        return app;
    }
}
