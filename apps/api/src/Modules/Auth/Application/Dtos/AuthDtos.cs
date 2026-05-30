namespace Api.Modules.Auth.Application.Dtos;

public sealed record AuthenticatedUserDto(Guid Id, string Email, string Name);
