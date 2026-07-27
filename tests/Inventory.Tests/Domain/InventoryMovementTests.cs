using FluentAssertions;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;

namespace Inventory.Tests.Domain;

public class InventoryMovementTests
{
    [Fact]
    public void Create_with_valid_data_succeeds()
    {
        var movement = new InventoryMovement(1, MovementType.Inbound, 5, "Entry #1", 10);

        movement.ProductId.Should().Be(1);
        movement.MovementType.Should().Be(MovementType.Inbound);
        movement.Quantity.Should().Be(5);
        movement.Notes.Should().Be("Entry #1");
        movement.ReferenceId.Should().Be(10);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_with_non_positive_quantity_throws(int quantity)
    {
        var act = () => new InventoryMovement(1, MovementType.Outbound, quantity);

        act.Should().Throw<DomainException>();
    }
}
