namespace Api.Modules.Logging;

public class LoggingOptions
{
    public const string SectionName = "Serilog";

    public LogLevelOptions? MinimumLevel { get; set; }
    public WriteToOptions[]? WriteTo { get; set; }
}

public class LogLevelOptions
{
    public string Default { get; set; } = "Information";
    public Dictionary<string, string>? Override { get; set; }
}

public class WriteToOptions
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string>? Args { get; set; }
}
