using Dapper;
using ProductService.Domain.Users;
using ProductService.Infrastructure.Database;

namespace ProductService.Infrastructure.Repositories;

public class UserRepository(IConnectionFactory connectionFactory) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id)
    {
        const string sql = "SELECT id, username, email FROM products_identity.users WHERE id = @id";

        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { id });
    }

    public async Task AddAsync(User user)
    {
        const string sql = @"
            INSERT INTO products_identity.users (username, email)
            VALUES (@Username, @Email);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = connectionFactory.CreateConnection();
        user.Id = await connection.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        const string sql = @"
            UPDATE products_identity.users
            SET username = @Username, email = @Email
            WHERE id = @Id";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, user);
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM products_identity.users WHERE id = @id";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { id });
    }
}
