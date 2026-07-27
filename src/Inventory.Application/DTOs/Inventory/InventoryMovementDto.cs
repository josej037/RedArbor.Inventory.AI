using Inventory.Domain.Enums;

namespace Inventory.Application.DTOs.Inventory;

public sealed class InventoryMovementDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public MovementType MovementType { get; init; }
    public int Quantity { get; init; }
    public string? Notes { get; init; }
    public int? ReferenceId { get; init; }
}
