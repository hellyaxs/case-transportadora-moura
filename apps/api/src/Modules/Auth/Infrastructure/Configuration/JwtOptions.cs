namespace Api.Modules.Auth.Infrastructure.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "transportadora-moura-api";
    public string Audience { get; set; } = "transportadora-moura-web";
    public int ExpirationHours { get; set; } = 8;
    public string CookieName { get; set; } = "auth_token";
}
