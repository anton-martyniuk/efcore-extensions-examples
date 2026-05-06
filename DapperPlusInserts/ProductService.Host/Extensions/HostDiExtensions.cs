using ProductService.Domain.Products;
using ProductService.Domain.Users;
using ProductService.Infrastructure.Database;
using ProductService.Infrastructure.Repositories;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class HostDiExtensions
{
    public static IServiceCollection AddWebHostInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductCartRepository, ProductCartRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddDapper(configuration);

        return services;
    }

    private static IServiceCollection AddDapper(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlServer")
                               ?? throw new InvalidOperationException("Missing connection string 'SqlServer'.");

        services.AddSingleton<IConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

        return services;
    }
}
