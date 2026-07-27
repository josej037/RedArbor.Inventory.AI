using Inventory.Domain.Exceptions;

namespace Inventory.Domain.Entities;

public class InventoryEntry
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public InventoryEntry()
    {
    }

    public InventoryEntry(int productId, int quantity, string? notes = null)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Inventory entry quantity must be greater than zero.");
        }

        ProductId = productId;
        Quantity = quantity;
        Notes = notes;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
