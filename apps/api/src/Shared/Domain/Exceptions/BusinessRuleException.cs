namespace Api.Shared.Domain.Exceptions;

public sealed class BusinessRuleException : InvalidOperationException
{
    public BusinessRuleException(string message, string code)
        : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
