using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Persistence.Seed;

public sealed class DemoDataSeeder(
    InventoryDbContext dbContext,
    ILogger<DemoDataSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var hasCategories = await dbContext.Categories.AnyAsync(cancellationToken);
        var hasProducts = await dbContext.Products.AnyAsync(cancellationToken);

        if (hasCategories || hasProducts)
        {
            logger.LogInformation("Demo seed skipped because Categories or Products already exist.");
            return;
        }

        var electronics = new Category("Electronics", "Electronic devices and accessories");
        var office = new Category("Office Supplies", "Stationery and office materials");
        var tools = new Category("Tools", "Hand and power tools");

        dbContext.Categories.AddRange(electronics, office, tools);
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.Products.AddRange(
            new Product(electronics.Id, "Wireless Mouse", 25, 19.99m, "Ergonomic wireless mouse"),
            new Product(electronics.Id, "USB-C Hub", 15, 49.50m, "4-port USB-C hub"),
            new Product(office.Id, "Notebook A5", 100, 3.25m, "Ruled notebook"),
            new Product(tools.Id, "Screwdriver Set", 40, 14.90m, "6-piece precision set"));

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Demo Categories and Products seeded successfully.");
    }
}
