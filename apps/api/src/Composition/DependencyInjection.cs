using Api.Modules.Auth;
using Api.Modules.Collections;
using Api.Modules.Logging;

namespace Api.Composition;

public static class DependencyInjection
{
    public static IServiceCollection AddApiModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCollectionsModule(configuration);
        services.AddAuthModule(configuration);
        services.AddLoggingModule(configuration);

        return services;
    }
}
