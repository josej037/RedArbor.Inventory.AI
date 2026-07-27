using Dapper;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class InventoryExitRepository(
    InventoryDbContext dbContext,
    ISqlConnectionFactory connectionFactory) : IInventoryExitRepository
{
    public async Task<InventoryExit?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.InventoryExits
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryExit>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.InventoryExits
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> AddAsync(InventoryExit exit, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO InventoryExits (ProductId, Quantity, Notes, CreatedAtUtc)
            OUTPUT INSERTED.Id
            VALUES (@ProductId, @Quantity, @Notes, @CreatedAtUtc);
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, exit, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM InventoryExits
            WHERE Id = @Id;
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}
