using Api.Application.Coletas.Contracts;
using Api.Application.Coletas.Dtos;
using Api.Application.Coletas.UseCases;
using Api.Domain.Coletas.Enums;
using Api.Domain.Coletas.Exceptions;

namespace Api.Presentation.Coletas;

public static class ColetasEndpoints
{
    public static RouteGroupBuilder MapColetasEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/coletas")
            .WithTags("Coletas");

        group.MapGet("/", ListarColetasAsync)
            .WithName("ListarColetas")
            .WithSummary("Lista coletas operacionais")
            .WithDescription("Lista coletas com filtros por status, cliente e período. Retorna também flags de prioridade e atraso para acompanhamento operacional.")
            .Produces<IReadOnlyList<ColetaResumoDto>>();

        group.MapGet("/{id:guid}", ObterColetaAsync)
            .WithName("ObterColeta")
            .WithSummary("Obtém detalhe de uma coleta")
            .WithDescription("Retorna dados completos da coleta, atribuição operacional e ocorrências em ordem cronológica.")
            .Produces<ColetaDetalheDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CriarColetaAsync)
            .WithName("CriarColeta")
            .WithSummary("Cria uma solicitação de coleta")
            .WithDescription("Cria uma coleta com status Aberta, número único e prioridade normal quando a prioridade não for informada.")
            .Produces<ColetaDetalheDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/{id:guid}/atribuicao", AtribuirColetaAsync)
            .WithName("AtribuirColeta")
            .WithSummary("Atribui motorista e veículo")
            .WithDescription("Vincula motorista e veículo a uma coleta ativa. Coletas canceladas não aceitam atribuição.")
            .Produces<ColetaDetalheDto>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/iniciar", IniciarColetaAsync)
            .WithName("IniciarColeta")
            .WithSummary("Marca uma coleta como EmColeta")
            .WithDescription("Avança uma coleta Aberta para EmColeta somente quando motorista e veículo já estão vinculados.")
            .Produces<ColetaDetalheDto>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/concluir", ConcluirColetaAsync)
            .WithName("ConcluirColeta")
            .WithSummary("Marca uma coleta como Coletada")
            .WithDescription("Conclui uma coleta em execução somente quando motorista e veículo estão vinculados.")
            .Produces<ColetaDetalheDto>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/cancelar", CancelarColetaAsync)
            .WithName("CancelarColeta")
            .WithSummary("Cancela uma coleta")
            .WithDescription("Cancela uma coleta ativa. O status Cancelada é terminal e não retorna ao fluxo operacional.")
            .Produces<ColetaDetalheDto>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/ocorrencias", RegistrarOcorrenciaAsync)
            .WithName("RegistrarOcorrencia")
            .WithSummary("Registra ocorrência operacional")
            .WithDescription("Registra ocorrência com descrição, data/hora do servidor e usuário responsável informado.")
            .Produces<ColetaDetalheDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return group;
    }

    public static IEndpointRouteBuilder MapCadastrosOperacionaisEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clientes", async (IColetaRepository repository, CancellationToken cancellationToken) =>
            await repository.ListClientesAsync(cancellationToken))
            .WithTags("Cadastros operacionais")
            .WithSummary("Lista clientes disponíveis para coletas")
            .Produces<IReadOnlyList<OptionDto>>();

        app.MapGet("/api/motoristas", async (IColetaRepository repository, CancellationToken cancellationToken) =>
            await repository.ListMotoristasAsync(cancellationToken))
            .WithTags("Cadastros operacionais")
            .WithSummary("Lista motoristas disponíveis para atribuição")
            .Produces<IReadOnlyList<OptionDto>>();

        app.MapGet("/api/veiculos", async (IColetaRepository repository, CancellationToken cancellationToken) =>
            await repository.ListVeiculosAsync(cancellationToken))
            .WithTags("Cadastros operacionais")
            .WithSummary("Lista veículos disponíveis para atribuição")
            .Produces<IReadOnlyList<VeiculoOptionDto>>();

        return app;
    }

    private static async Task<IResult> ListarColetasAsync(
        ColetaUseCases useCases,
        ColetaStatus? status,
        Guid? clienteId,
        DateOnly? dataInicial,
        DateOnly? dataFinal,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.ListarAsync(status, clienteId, dataInicial, dataFinal, cancellationToken)));
    }

    private static async Task<IResult> ObterColetaAsync(
        Guid id,
        ColetaUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.ObterDetalheAsync(id, cancellationToken)));
    }

    private static async Task<IResult> CriarColetaAsync(
        CriarColetaRequest request,
        ColetaUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var coleta = await useCases.CriarAsync(request.ToDto(), cancellationToken);
            return Results.Created($"/api/coletas/{coleta.Id}", coleta);
        });
    }

    private static async Task<IResult> AtribuirColetaAsync(
        Guid id,
        AtribuirColetaRequest request,
        ColetaUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.AtribuirAsync(id, request.MotoristaId, request.VeiculoId, cancellationToken)));
    }

    private static async Task<IResult> IniciarColetaAsync(
        Guid id,
        ColetaUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.MarcarEmColetaAsync(id, cancellationToken)));
    }

    private static async Task<IResult> ConcluirColetaAsync(
        Guid id,
        ColetaUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.ConcluirAsync(id, cancellationToken)));
    }

    private static async Task<IResult> CancelarColetaAsync(
        Guid id,
        CancelarColetaRequest request,
        ColetaUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.CancelarAsync(id, request.Motivo, cancellationToken)));
    }

    private static async Task<IResult> RegistrarOcorrenciaAsync(
        Guid id,
        RegistrarOcorrenciaRequest request,
        ColetaUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.RegistrarOcorrenciaAsync(id, request.Descricao, request.UsuarioResponsavel, cancellationToken)));
    }

    private static async Task<IResult> ExecuteAsync(Func<Task<IResult>> action)
    {
        try
        {
            return await action();
        }
        catch (BusinessRuleException exception)
        {
            return Results.Problem(
                title: "Regra de negócio violada",
                detail: exception.Message,
                statusCode: StatusCodes.Status409Conflict,
                extensions: new Dictionary<string, object?> { ["code"] = exception.Code });
        }
        catch (KeyNotFoundException exception)
        {
            return Results.Problem(
                title: "Recurso não encontrado",
                detail: exception.Message,
                statusCode: StatusCodes.Status404NotFound);
        }
        catch (ArgumentException exception)
        {
            return Results.Problem(
                title: "Requisição inválida",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
