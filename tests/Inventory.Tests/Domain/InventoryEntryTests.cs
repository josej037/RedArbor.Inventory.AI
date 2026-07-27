using FluentAssertions;
using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;

namespace Inventory.Tests.Domain;

public class InventoryEntryTests
{
    [Fact]
    public void Create_with_valid_data_succeeds()
    {
        var entry = new InventoryEntry(1, 5, "Restock");

        entry.ProductId.Should().Be(1);
        entry.Quantity.Should().Be(5);
        entry.Notes.Should().Be("Restock");
        entry.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_with_non_positive_quantity_throws(int quantity)
    {
        var act = () => new InventoryEntry(1, quantity);

        act.Should().Throw<DomainException>();
    }
}
