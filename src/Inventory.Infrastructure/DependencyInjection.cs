using Inventory.Application.Abstractions.Persistence;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("InventoryDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'InventoryDb' is not configured.");
        }

        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IInventoryEntryRepository, InventoryEntryRepository>();
        services.AddScoped<IInventoryExitRepository, InventoryExitRepository>();
        services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();

        return services;
    }
}
