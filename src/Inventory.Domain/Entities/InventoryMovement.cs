using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;

namespace Inventory.Domain.Entities;

public class InventoryMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public MovementType MovementType { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public int? ReferenceId { get; set; }

    public InventoryMovement()
    {
    }

    public InventoryMovement(
        int productId,
        MovementType movementType,
        int quantity,
        string? notes = null,
        int? referenceId = null)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Inventory movement quantity must be greater than zero.");
        }

        ProductId = productId;
        MovementType = movementType;
        Quantity = quantity;
        Notes = notes;
        ReferenceId = referenceId;
    }
}
