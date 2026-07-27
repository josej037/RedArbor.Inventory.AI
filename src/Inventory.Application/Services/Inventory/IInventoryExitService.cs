using Inventory.Application.DTOs.Inventory;

namespace Inventory.Application.Services.Inventory;

public interface IInventoryExitService
{
    Task<InventoryExitDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryExitDto>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateInventoryExitRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
