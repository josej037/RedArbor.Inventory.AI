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

    public async Task<int> CreateWithStockAndMovementAsync(
        InventoryExit exit,
        Product product,
        InventoryMovement movement,
        CancellationToken cancellationToken = default)
    {
        const string insertExitSql = """
            INSERT INTO InventoryExits (ProductId, Quantity, Notes, CreatedAtUtc)
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
            var exitId = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(insertExitSql, exit, transaction, cancellationToken: cancellationToken));

            movement.ReferenceId = exitId;

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
            return exitId;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeleteWithStockAsync(
        int exitId,
        Product product,
        CancellationToken cancellationToken = default)
    {
        const string deleteExitSql = """
            DELETE FROM InventoryExits
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
                    deleteExitSql,
                    new { Id = exitId },
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
