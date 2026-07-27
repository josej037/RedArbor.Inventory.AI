namespace Inventory.Application.DTOs.Products;

public sealed class UpdateProductRequest
{
    public int CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Stock { get; init; }
    public decimal UnitPrice { get; init; }
}
