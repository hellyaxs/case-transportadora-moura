namespace Api.Shared.Presentation;

public static class ResultExtensions
{
    public static IResult WithCookie(this IResult result, string name, string value, CookieOptions options)
    {
        return new CookieResult(result, name, value, options);
    }

    private sealed class CookieResult : IResult
    {
        private readonly IResult _inner;
        private readonly string _name;
        private readonly string _value;
        private readonly CookieOptions _options;

        public CookieResult(IResult inner, string name, string value, CookieOptions options)
        {
            _inner = inner;
            _name = name;
            _value = value;
            _options = options;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append(_name, _value, _options);
            await _inner.ExecuteAsync(httpContext);
        }
    }
}
