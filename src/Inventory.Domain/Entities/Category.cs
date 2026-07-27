using Inventory.Domain.Exceptions;

namespace Inventory.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public Category()
    {
    }

    public Category(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Category name is required.");
        }

        Name = name.Trim();
        Description = description;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
