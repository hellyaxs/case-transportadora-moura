using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Api.Shared.Infrastructure.Configuration;

public static class PostgresConnectionStringFactory
{
    public static string Create(IConfiguration configuration)
    {
        var configuredConnectionString = configuration.GetConnectionString("Transportadora");
        if (!string.IsNullOrWhiteSpace(configuredConnectionString))
        {
            return configuredConnectionString;
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = configuration["POSTGRES_HOST"] ?? "localhost",
            Port = int.TryParse(configuration["POSTGRES_PORT"], out var port) ? port : 5432,
            Database = configuration["POSTGRES_DB"] ?? "transportadora_moura",
            Username = configuration["POSTGRES_USER"] ?? "transportadora",
            Password = configuration["POSTGRES_PASSWORD"] ?? "transportadora",
            IncludeErrorDetail = configuration.GetValue("POSTGRES_INCLUDE_ERROR_DETAIL", true),
        };

        return builder.ConnectionString;
    }
}
