using Api.Modules.Auth;
using Api.Modules.Collections;

namespace Api.Composition;

public static class DependencyInjection
{
    public static IServiceCollection AddApiModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCollectionsModule(configuration);
        services.AddAuthModule(configuration);

        return services;
    }
}
