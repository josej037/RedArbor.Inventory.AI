using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Persistence;

public interface IInventoryMovementRepository
{
    Task<InventoryMovement?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryMovement>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryMovement>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);

    Task<int> AddAsync(InventoryMovement movement, CancellationToken cancellationToken = default);
}
