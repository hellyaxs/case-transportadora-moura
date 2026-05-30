using Api.Modules.Auth.Presentation;
using Api.Modules.Auth.Presentation.Validators;

namespace Api.Test.Modules.Auth.Application;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldPass()
    {
        var result = await _validator.ValidateAsync(new LoginRequest("operador@moura.local", "Moura@2026"));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Moura@2026")]
    [InlineData("invalid-email", "Moura@2026")]
    [InlineData("operador@moura.local", "")]
    [InlineData("operador@moura.local", "123")]
    public async Task Validate_WithInvalidRequest_ShouldFail(string email, string password)
    {
        var result = await _validator.ValidateAsync(new LoginRequest(email, password));

        Assert.False(result.IsValid);
    }
}
