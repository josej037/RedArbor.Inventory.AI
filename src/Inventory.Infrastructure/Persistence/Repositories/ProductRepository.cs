using Dapper;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class ProductRepository(
    InventoryDbContext dbContext,
    ISqlConnectionFactory connectionFactory) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Products (CategoryId, Name, Description, Stock, UnitPrice, CreatedAtUtc, UpdatedAtUtc)
            OUTPUT INSERTED.Id
            VALUES (@CategoryId, @Name, @Description, @Stock, @UnitPrice, @CreatedAtUtc, @UpdatedAtUtc);
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, product, cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Products
            SET CategoryId = @CategoryId,
                Name = @Name,
                Description = @Description,
                Stock = @Stock,
                UnitPrice = @UnitPrice,
                UpdatedAtUtc = @UpdatedAtUtc
            WHERE Id = @Id;
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(sql, product, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM Products
            WHERE Id = @Id;
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}
