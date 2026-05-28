using Api.Application.Coletas.Contracts;
using Api.Application.Coletas.UseCases;
using Api.Domain.Coletas.Enums;
using Api.Infrastructure.Configuration;
using Api.Infrastructure.Coletas;
using Api.Infrastructure.Coletas.Persistence;
using Api.Infrastructure.Coletas.Repositories;
using Api.Presentation.Coletas;
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

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
    options.MapType<ColetaStatus>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames<ColetaStatus>().Select(name => new OpenApiString(name)).Cast<IOpenApiAny>().ToList(),
    });
    options.MapType<ColetaPrioridade>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames<ColetaPrioridade>().Select(name => new OpenApiString(name)).Cast<IOpenApiAny>().ToList(),
    });
});
builder.Services.AddCors(options =>
{
    var allowedOrigins = (builder.Configuration["FRONTEND_ORIGIN"] ?? "http://localhost:5173")
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddDbContext<TransportadoraDbContext>(options =>
    options.UseNpgsql(PostgresConnectionStringFactory.Create(builder.Configuration)));
builder.Services.AddScoped<IColetaRepository, EfColetaRepository>();
builder.Services.AddScoped<ColetaUseCases>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IColetaNumeroGenerator, ColetaNumeroGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TransportadoraDbContext>();
    dbContext.Database.Migrate();
}

app.UseCors();
app.UseHttpsRedirection();
app.MapColetasEndpoints();
app.MapCadastrosOperacionaisEndpoints();

app.Run();

public partial class Program;
