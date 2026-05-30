using Api.Modules.Auth.Domain.Entities;

namespace Api.Modules.Auth.Application.Contracts;

public interface IJwtTokenService
{
    string CreateToken(User user);
}
