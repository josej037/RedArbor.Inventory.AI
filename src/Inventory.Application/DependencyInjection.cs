using Inventory.Application.Services.Auth;
using Inventory.Application.Services.Categories;
using Inventory.Application.Services.Inventory;
using Inventory.Application.Services.Products;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IInventoryEntryService, InventoryEntryService>();
        services.AddScoped<IInventoryExitService, InventoryExitService>();
        services.AddScoped<IInventoryMovementService, InventoryMovementService>();

        return services;
    }
}
