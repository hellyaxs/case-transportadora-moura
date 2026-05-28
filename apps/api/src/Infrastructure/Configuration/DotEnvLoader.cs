namespace Api.Infrastructure.Configuration;

public static class DotEnvLoader
{
    public static void Load(string fileName = ".env")
    {
        foreach (var path in CandidatePaths(fileName).Distinct())
        {
            if (File.Exists(path))
            {
                LoadFile(path);
                return;
            }
        }
    }

    private static IEnumerable<string> CandidatePaths(string fileName)
    {
        var current = Directory.GetCurrentDirectory();

        yield return Path.Combine(current, fileName);
        yield return Path.Combine(current, "apps", "api", fileName);
        yield return Path.Combine(AppContext.BaseDirectory, fileName);

        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            yield return Path.Combine(directory.FullName, fileName);
            yield return Path.Combine(directory.FullName, "apps", "api", fileName);
            directory = directory.Parent;
        }
    }

    private static void LoadFile(string path)
    {
        foreach (var rawLine in File.ReadAllLines(path))
        {
            var line = rawLine.Trim();

            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim().Trim('"');

            if (Environment.GetEnvironmentVariable(key) is null)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}
