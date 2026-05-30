using Api.Composition;
using Api.Modules.Auth;
using Api.Modules.Collections.Infrastructure.Persistence;
using Api.Modules.Collections.Domain.Enums;
using Api.Shared.Infrastructure.Configuration;
using Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

DotEnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

var apiHttpPort = builder.Configuration["API_HTTP_PORT"];
if (!string.IsNullOrWhiteSpace(apiHttpPort) && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls($"http://localhost:{apiHttpPort}");
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Transportadora Moura - Gestão de Coletas",
        Version = "v1",
        Description = "API para cadastro, roteirização, ocorrências e acompanhamento de solicitações de coleta.",
    });
    options.MapType<CollectionStatus>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames<CollectionStatus>().Select(name => new OpenApiString(name)).Cast<IOpenApiAny>().ToList(),
    });
    options.MapType<CollectionPriority>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames<CollectionPriority>().Select(name => new OpenApiString(name)).Cast<IOpenApiAny>().ToList(),
    });
});
builder.Services.AddCors(options =>
{
    var allowedOrigins = (builder.Configuration["FRONTEND_ORIGIN"] ?? "http://localhost:5173")
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddApiModules(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TransportadoraDbContext>();
    dbContext.Database.Migrate();
    CollectionSeedData.Seed(dbContext);
}

app.SeedAuthData();
app.UseMiddleware<Api.Shared.Presentation.ExceptionHandlingMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapApiModules();

app.Run();

public partial class Program;
