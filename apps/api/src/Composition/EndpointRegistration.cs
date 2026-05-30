using Api.Modules.Collections;

namespace Api.Composition;

public static class EndpointRegistration
{
    public static WebApplication MapApiModules(this WebApplication app)
    {
        app.MapCollectionsModule();

        return app;
    }
}
