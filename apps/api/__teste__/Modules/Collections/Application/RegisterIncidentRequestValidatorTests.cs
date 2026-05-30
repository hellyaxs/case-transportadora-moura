using Api.Modules.Collections.Presentation;
using Api.Modules.Collections.Presentation.Validators;

namespace Api.Test.Modules.Collections.Application;

public class RegisterIncidentRequestValidatorTests
{
    private readonly RegisterIncidentRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldPass()
    {
        var result = await _validator.ValidateAsync(new RegisterIncidentRequest("Customer unavailable at pickup window."));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_WithEmptyDescription_ShouldFail(string description)
    {
        var result = await _validator.ValidateAsync(new RegisterIncidentRequest(description));

        Assert.False(result.IsValid);
    }
}
