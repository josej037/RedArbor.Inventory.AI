using Inventory.Application.DTOs.Inventory;

namespace Inventory.Application.Services.Inventory;

public interface IInventoryEntryService
{
    Task<InventoryEntryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryEntryDto>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateInventoryEntryRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
