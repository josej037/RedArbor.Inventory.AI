using Inventory.Domain.Exceptions;

namespace Inventory.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Stock { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public Product()
    {
    }

    public Product(int categoryId, string name, int stock, decimal unitPrice, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Product name is required.");
        }

        if (stock < 0)
        {
            throw new DomainException("Product stock cannot be negative.");
        }

        if (unitPrice < 0)
        {
            throw new DomainException("Product unit price cannot be negative.");
        }

        CategoryId = categoryId;
        Name = name.Trim();
        Stock = stock;
        UnitPrice = unitPrice;
        Description = description;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
