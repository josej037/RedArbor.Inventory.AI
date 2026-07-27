using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Inventory;
using Inventory.Domain.Entities;

namespace Inventory.Application.Services.Inventory;

public class InventoryMovementService(IInventoryMovementRepository inventoryMovementRepository)
    : IInventoryMovementService
{
    public async Task<IReadOnlyList<InventoryMovementDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var movements = await inventoryMovementRepository.GetAllAsync(cancellationToken);
        return movements.Select(MapToDto).ToList();
    }

    public async Task<InventoryMovementDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var movement = await inventoryMovementRepository.GetByIdAsync(id, cancellationToken);
        return movement is null ? null : MapToDto(movement);
    }

    private static InventoryMovementDto MapToDto(InventoryMovement movement) =>
        new()
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            MovementType = movement.MovementType,
            Quantity = movement.Quantity,
            Notes = movement.Notes,
            ReferenceId = movement.ReferenceId
        };
}
