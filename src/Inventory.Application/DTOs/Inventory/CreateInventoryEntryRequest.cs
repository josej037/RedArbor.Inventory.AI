namespace Inventory.Application.DTOs.Inventory;

public sealed class CreateInventoryEntryRequest
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public string? Notes { get; init; }
}
