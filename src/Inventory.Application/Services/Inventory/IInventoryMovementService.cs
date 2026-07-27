using Inventory.Application.DTOs.Inventory;

namespace Inventory.Application.Services.Inventory;

public interface IInventoryMovementService
{
    Task<IReadOnlyList<InventoryMovementDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<InventoryMovementDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
