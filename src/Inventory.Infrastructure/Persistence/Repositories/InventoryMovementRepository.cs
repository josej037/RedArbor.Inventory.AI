using Dapper;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class InventoryMovementRepository(
    InventoryDbContext dbContext,
    ISqlConnectionFactory connectionFactory) : IInventoryMovementRepository
{
    public async Task<InventoryMovement?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.InventoryMovements
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryMovement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.InventoryMovements
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryMovement>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.InventoryMovements
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> AddAsync(InventoryMovement movement, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO InventoryMovements (ProductId, MovementType, Quantity, Notes, ReferenceId)
            OUTPUT INSERTED.Id
            VALUES (@ProductId, @MovementType, @Quantity, @Notes, @ReferenceId);
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, movement, cancellationToken: cancellationToken));
    }
}
