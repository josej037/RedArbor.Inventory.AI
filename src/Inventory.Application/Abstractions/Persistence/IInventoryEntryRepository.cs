using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Persistence;

public interface IInventoryEntryRepository
{
    Task<InventoryEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryEntry>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);

    Task<int> AddAsync(InventoryEntry entry, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
