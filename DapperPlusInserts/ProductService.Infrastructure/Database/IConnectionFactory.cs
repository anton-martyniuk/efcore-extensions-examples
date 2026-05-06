using System.Data;

namespace ProductService.Infrastructure.Database;

public interface IConnectionFactory
{
    IDbConnection CreateConnection();
}
