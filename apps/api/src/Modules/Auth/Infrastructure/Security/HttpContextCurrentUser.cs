using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Api.Shared.Application.Contracts;

namespace Api.Modules.Auth.Infrastructure.Security;

public sealed class HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    public string? Email => Principal?.FindFirstValue(ClaimTypes.Email)
        ?? Principal?.FindFirstValue(JwtRegisteredClaimNames.Email);

    public string? DisplayName => Principal?.FindFirstValue(ClaimTypes.Name);

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;
}
