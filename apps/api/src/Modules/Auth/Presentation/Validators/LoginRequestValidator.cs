using Api.Modules.Auth.Presentation;
using FluentValidation;

namespace Api.Modules.Auth.Presentation.Validators;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(160);

        RuleFor(request => request.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(120);
    }
}
