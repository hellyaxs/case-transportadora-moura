using FluentValidation;
using FluentValidation.Results;

namespace Api.Shared.Presentation;

public static class EndpointValidation
{
    public static async Task<IResult?> ValidateAsync<T>(
        T request,
        IValidator<T> validator,
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);
        return result.IsValid ? null : ToValidationProblem(result);
    }

    public static IResult ToValidationProblem(ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        return Results.ValidationProblem(errors, title: "Validation failed");
    }
}
