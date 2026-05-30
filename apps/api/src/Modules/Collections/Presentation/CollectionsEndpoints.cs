using Api.Modules.Collections.Application.Contracts;
using Api.Modules.Collections.Application.Dtos;
using Api.Modules.Collections.Application.UseCases;
using Api.Modules.Collections.Domain.Enums;
using Api.Modules.Collections.Presentation.Validators;
using Api.Shared.Domain.Exceptions;
using Api.Shared.Presentation;
using FluentValidation;

namespace Api.Modules.Collections.Presentation;

public static class CollectionsEndpoints
{
    public static RouteGroupBuilder MapCollectionsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/collections")
            .WithTags("Collections")
            .RequireAuthorization();

        group.MapGet("/", ListCollectionsAsync)
            .WithName("ListCollections")
            .WithSummary("List operational collections")
            .WithDescription("Lists collections with filters by status, customer, and date range. Returns priority and overdue flags for operational tracking.")
            .Produces<IReadOnlyList<CollectionSummaryDto>>();

        group.MapGet("/{id:guid}", GetCollectionAsync)
            .WithName("GetCollection")
            .WithSummary("Get collection details")
            .WithDescription("Returns full collection data, operational assignment, and incidents in chronological order.")
            .Produces<CollectionDetailsDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateCollectionAsync)
            .WithName("CreateCollection")
            .WithSummary("Create a collection request")
            .WithDescription("Creates a collection with Open status, a unique number, and Normal priority when priority is not provided.")
            .Produces<CollectionDetailsDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/{id:guid}/assignment", AssignCollectionAsync)
            .WithName("AssignCollection")
            .WithSummary("Assign driver and vehicle")
            .WithDescription("Links a driver and vehicle to an active collection. Cancelled collections cannot be assigned.")
            .Produces<CollectionDetailsDto>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/start", StartCollectionAsync)
            .WithName("StartCollection")
            .WithSummary("Mark a collection as InProgress")
            .WithDescription("Advances an Open collection to InProgress only when a driver and vehicle are already assigned.")
            .Produces<CollectionDetailsDto>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/complete", CompleteCollectionAsync)
            .WithName("CompleteCollection")
            .WithSummary("Mark a collection as Collected")
            .WithDescription("Completes an in-progress collection only when a driver and vehicle are assigned.")
            .Produces<CollectionDetailsDto>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/cancel", CancelCollectionAsync)
            .WithName("CancelCollection")
            .WithSummary("Cancel a collection")
            .WithDescription("Cancels an active collection. Cancelled status is terminal and does not return to the operational flow.")
            .Produces<CollectionDetailsDto>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/incidents", RegisterIncidentAsync)
            .WithName("RegisterIncident")
            .WithSummary("Register an operational incident")
            .WithDescription("Registers an incident with description, server timestamp, and the authenticated responsible user.")
            .Produces<CollectionDetailsDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return group;
    }

    public static IEndpointRouteBuilder MapOperationalCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customers", async (ICollectionRepository repository, CancellationToken cancellationToken) =>
            await repository.ListCustomersAsync(cancellationToken))
            .WithTags("Operational catalog")
            .WithSummary("List customers available for collections")
            .Produces<IReadOnlyList<OptionDto>>()
            .RequireAuthorization();

        app.MapGet("/api/drivers", async (ICollectionRepository repository, CancellationToken cancellationToken) =>
            await repository.ListDriversAsync(cancellationToken))
            .WithTags("Operational catalog")
            .WithSummary("List drivers available for assignment")
            .Produces<IReadOnlyList<OptionDto>>()
            .RequireAuthorization();

        app.MapGet("/api/vehicles", async (ICollectionRepository repository, CancellationToken cancellationToken) =>
            await repository.ListVehiclesAsync(cancellationToken))
            .WithTags("Operational catalog")
            .WithSummary("List vehicles available for assignment")
            .Produces<IReadOnlyList<VehicleOptionDto>>()
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> ListCollectionsAsync(
        CollectionUseCases useCases,
        CollectionStatus? status,
        Guid? customerId,
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.ListAsync(status, customerId, startDate, endDate, cancellationToken)));
    }

    private static async Task<IResult> GetCollectionAsync(
        Guid id,
        CollectionUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.GetDetailsAsync(id, cancellationToken)));
    }

    private static async Task<IResult> CreateCollectionAsync(
        CreateCollectionRequest request,
        CollectionUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var collection = await useCases.CreateAsync(request.ToDto(), cancellationToken);
            return Results.Created($"/api/collections/{collection.Id}", collection);
        });
    }

    private static async Task<IResult> AssignCollectionAsync(
        Guid id,
        AssignCollectionRequest request,
        CollectionUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.AssignAsync(id, request.DriverId, request.VehicleId, cancellationToken)));
    }

    private static async Task<IResult> StartCollectionAsync(
        Guid id,
        CollectionUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.MarkInProgressAsync(id, cancellationToken)));
    }

    private static async Task<IResult> CompleteCollectionAsync(
        Guid id,
        CollectionUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.CompleteAsync(id, cancellationToken)));
    }

    private static async Task<IResult> CancelCollectionAsync(
        Guid id,
        CancelCollectionRequest request,
        CollectionUseCases useCases,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.CancelAsync(id, request.Reason, cancellationToken)));
    }

    private static async Task<IResult> RegisterIncidentAsync(
        Guid id,
        RegisterIncidentRequest request,
        IValidator<RegisterIncidentRequest> validator,
        CollectionUseCases useCases,
        CancellationToken cancellationToken)
    {
        var validationError = await EndpointValidation.ValidateAsync(request, validator, cancellationToken);
        if (validationError is not null)
        {
            return validationError;
        }

        return await ExecuteAsync(async () =>
            Results.Ok(await useCases.RegisterIncidentAsync(id, request.Description, cancellationToken)));
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
                title: "Business rule violated",
                detail: exception.Message,
                statusCode: StatusCodes.Status409Conflict,
                extensions: new Dictionary<string, object?> { ["code"] = exception.Code });
        }
        catch (KeyNotFoundException exception)
        {
            return Results.Problem(
                title: "Resource not found",
                detail: exception.Message,
                statusCode: StatusCodes.Status404NotFound);
        }
        catch (ArgumentException exception)
        {
            return Results.Problem(
                title: "Invalid request",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
