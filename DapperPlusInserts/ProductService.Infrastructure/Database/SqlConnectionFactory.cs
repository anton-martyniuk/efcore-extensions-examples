using System.Data;
using Microsoft.Data.SqlClient;

namespace ProductService.Infrastructure.Database;

public sealed class SqlConnectionFactory(string connectionString) : IConnectionFactory
{
    private readonly string _connectionString =
        connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    public IDbConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
