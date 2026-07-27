using FluentAssertions;
using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;

namespace Inventory.Tests.Domain;

public class InventoryExitTests
{
    [Fact]
    public void Create_with_valid_data_succeeds()
    {
        var exit = new InventoryExit(1, 3, "Sale");

        exit.ProductId.Should().Be(1);
        exit.Quantity.Should().Be(3);
        exit.Notes.Should().Be("Sale");
        exit.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_with_non_positive_quantity_throws(int quantity)
    {
        var act = () => new InventoryExit(1, quantity);

        act.Should().Throw<DomainException>();
    }
}
