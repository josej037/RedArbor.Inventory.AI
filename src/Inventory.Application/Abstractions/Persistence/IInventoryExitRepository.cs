using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Persistence;

public interface IInventoryExitRepository
{
    Task<InventoryExit?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryExit>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);

    Task<int> AddAsync(InventoryExit exit, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateWithStockAndMovementAsync(
        InventoryExit exit,
        Product product,
        InventoryMovement movement,
        CancellationToken cancellationToken = default);

    Task DeleteWithStockAsync(
        int exitId,
        Product product,
        CancellationToken cancellationToken = default);
}
