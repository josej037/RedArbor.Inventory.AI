namespace Inventory.Application.DTOs.Categories;

public sealed class UpdateCategoryRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}
