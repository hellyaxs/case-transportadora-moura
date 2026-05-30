using Api.Modules.Auth;
using Api.Modules.Collections;

namespace Api.Composition;

public static class EndpointRegistration
{
    public static WebApplication MapApiModules(this WebApplication app)
    {
        app.MapCollectionsModule();
        app.MapAuthModule();

        return app;
    }
}
