using FluentAssertions;
using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;

namespace Inventory.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_with_valid_data_succeeds()
    {
        var product = new Product(1, "Laptop", 10, 999.99m, "Portable PC");

        product.CategoryId.Should().Be(1);
        product.Name.Should().Be("Laptop");
        product.Stock.Should().Be(10);
        product.UnitPrice.Should().Be(999.99m);
        product.Description.Should().Be("Portable PC");
        product.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        product.UpdatedAtUtc.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_with_invalid_name_throws(string? name)
    {
        var act = () => new Product(1, name!, 0, 1m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_with_negative_stock_throws()
    {
        var act = () => new Product(1, "Laptop", -1, 1m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_with_negative_unit_price_throws()
    {
        var act = () => new Product(1, "Laptop", 0, -1m);

        act.Should().Throw<DomainException>();
    }
}
