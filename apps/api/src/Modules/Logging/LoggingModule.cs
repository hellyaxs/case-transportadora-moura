using Serilog;
using Serilog.Events;

namespace Api.Modules.Logging;

public static class LoggingModule
{
    public static IServiceCollection AddLoggingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LoggingOptions>(configuration.GetSection(LoggingOptions.SectionName));
        return services;
    }

    public static WebApplication UseLoggingModule(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);
            };

            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (httpContext.Request.Path.StartsWithSegments("/swagger")
                    || httpContext.Request.Path.StartsWithSegments("/favicon.ico"))
                {
                    return LogEventLevel.Verbose;
                }

                return LogEventLevel.Information;
            };
        });

        return app;
    }
}
