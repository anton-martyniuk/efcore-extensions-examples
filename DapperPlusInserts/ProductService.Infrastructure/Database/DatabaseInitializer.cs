using System.Reflection;
using Dapper;
using ProductService.Domain.Users;
using Z.Dapper.Plus;
using Z.Dapper.Sql;

namespace ProductService.Infrastructure.Database;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IConnectionFactory connectionFactory)
    {
        var script = LoadScript("ProductService.Infrastructure.Database.SqlScripts.CreateSchema.sql");

        using var connection = connectionFactory.CreateConnection();

        foreach (var batch in SplitOnGo(script))
        {
            if (string.IsNullOrWhiteSpace(batch))
            {
                continue;
            }

            await connection.ExecuteAsync(batch);
        }
    }

    private static string LoadScript(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName)
                          ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' was not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static IEnumerable<string> SplitOnGo(string script)
    {
        return script.Split(["\nGO", "\rGO", "\nGo", "\ngo"], StringSplitOptions.None);
    }
}
