using FluentAssertions;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.Services.Inventory;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Moq;

namespace Inventory.Tests.Application;

public class InventoryMovementServiceTests
{
    private readonly Mock<IInventoryMovementRepository> _movementRepository = new();
    private readonly InventoryMovementService _sut;

    public InventoryMovementServiceTests()
    {
        _sut = new InventoryMovementService(_movementRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_returns_mapped_dtos()
    {
        var movements = new List<InventoryMovement>
        {
            new(1, MovementType.Inbound, 5, "Entry") { Id = 10, ReferenceId = 3 },
            new(2, MovementType.Outbound, 2, "Exit") { Id = 11, ReferenceId = 4 }
        };

        _movementRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(movements);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
        result[0].Id.Should().Be(10);
        result[0].ProductId.Should().Be(1);
        result[0].MovementType.Should().Be(MovementType.Inbound);
        result[0].Quantity.Should().Be(5);
        result[0].Notes.Should().Be("Entry");
        result[0].ReferenceId.Should().Be(3);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_when_missing()
    {
        _movementRepository
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryMovement?)null);

        var result = await _sut.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_returns_dto_when_found()
    {
        var movement = new InventoryMovement(7, MovementType.Inbound, 3, "Restock")
        {
            Id = 20,
            ReferenceId = 8
        };

        _movementRepository
            .Setup(x => x.GetByIdAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movement);

        var result = await _sut.GetByIdAsync(20);

        result.Should().NotBeNull();
        result!.Id.Should().Be(20);
        result.ProductId.Should().Be(7);
        result.MovementType.Should().Be(MovementType.Inbound);
        result.Quantity.Should().Be(3);
        result.Notes.Should().Be("Restock");
        result.ReferenceId.Should().Be(8);
    }
}
