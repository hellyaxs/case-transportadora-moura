using Api.Modules.Collections.Presentation;
using FluentValidation;

namespace Api.Modules.Collections.Presentation.Validators;

public sealed class RegisterIncidentRequestValidator : AbstractValidator<RegisterIncidentRequest>
{
    public RegisterIncidentRequestValidator()
    {
        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(500);
    }
}
