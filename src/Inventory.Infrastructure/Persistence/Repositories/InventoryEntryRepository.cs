using Dapper;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class InventoryEntryRepository(
    InventoryDbContext dbContext,
    ISqlConnectionFactory connectionFactory) : IInventoryEntryRepository
{
    public async Task<InventoryEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.InventoryEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryEntry>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.InventoryEntries
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> AddAsync(InventoryEntry entry, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO InventoryEntries (ProductId, Quantity, Notes, CreatedAtUtc)
            OUTPUT INSERTED.Id
            VALUES (@ProductId, @Quantity, @Notes, @CreatedAtUtc);
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, entry, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM InventoryEntries
            WHERE Id = @Id;
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}
