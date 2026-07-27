namespace Inventory.Application.DTOs.Inventory;

public sealed class InventoryEntryDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
