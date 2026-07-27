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

    public async Task<int> CreateWithStockAndMovementAsync(
        InventoryEntry entry,
        Product product,
        InventoryMovement movement,
        CancellationToken cancellationToken = default)
    {
        const string insertEntrySql = """
            INSERT INTO InventoryEntries (ProductId, Quantity, Notes, CreatedAtUtc)
            OUTPUT INSERTED.Id
            VALUES (@ProductId, @Quantity, @Notes, @CreatedAtUtc);
            """;

        const string insertMovementSql = """
            INSERT INTO InventoryMovements (ProductId, MovementType, Quantity, Notes, ReferenceId)
            OUTPUT INSERTED.Id
            VALUES (@ProductId, @MovementType, @Quantity, @Notes, @ReferenceId);
            """;

        const string updateProductSql = """
            UPDATE Products
            SET Stock = @Stock,
                UpdatedAtUtc = @UpdatedAtUtc
            WHERE Id = @Id;
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var entryId = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(insertEntrySql, entry, transaction, cancellationToken: cancellationToken));

            movement.ReferenceId = entryId;

            await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    insertMovementSql,
                    new
                    {
                        movement.ProductId,
                        MovementType = (int)movement.MovementType,
                        movement.Quantity,
                        movement.Notes,
                        movement.ReferenceId
                    },
                    transaction,
                    cancellationToken: cancellationToken));

            await connection.ExecuteAsync(
                new CommandDefinition(updateProductSql, product, transaction, cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);
            return entryId;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeleteWithStockAsync(
        int entryId,
        Product product,
        CancellationToken cancellationToken = default)
    {
        const string deleteEntrySql = """
            DELETE FROM InventoryEntries
            WHERE Id = @Id;
            """;

        const string updateProductSql = """
            UPDATE Products
            SET Stock = @Stock,
                UpdatedAtUtc = @UpdatedAtUtc
            WHERE Id = @Id;
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    deleteEntrySql,
                    new { Id = entryId },
                    transaction,
                    cancellationToken: cancellationToken));

            await connection.ExecuteAsync(
                new CommandDefinition(updateProductSql, product, transaction, cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
