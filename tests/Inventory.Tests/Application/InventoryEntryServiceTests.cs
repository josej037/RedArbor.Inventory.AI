using FluentAssertions;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Inventory;
using Inventory.Application.Exceptions;
using Inventory.Application.Services.Inventory;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Moq;

namespace Inventory.Tests.Application;

public class InventoryEntryServiceTests
{
    private readonly Mock<IInventoryEntryRepository> _entryRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly InventoryEntryService _sut;

    public InventoryEntryServiceTests()
    {
        _sut = new InventoryEntryService(_entryRepository.Object, _productRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_throws_NotFound_when_product_missing()
    {
        _productRepository
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var act = () => _sut.CreateAsync(new CreateInventoryEntryRequest
        {
            ProductId = 99,
            Quantity = 5
        });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_increases_stock_and_creates_inbound_movement()
    {
        var product = new Product(1, "Laptop", 10, 100m) { Id = 1 };
        _productRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _entryRepository
            .Setup(x => x.CreateWithStockAndMovementAsync(
                It.IsAny<InventoryEntry>(),
                It.IsAny<Product>(),
                It.IsAny<InventoryMovement>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(55);

        var id = await _sut.CreateAsync(new CreateInventoryEntryRequest
        {
            ProductId = 1,
            Quantity = 5,
            Notes = "Restock"
        });

        id.Should().Be(55);
        _entryRepository.Verify(
            x => x.CreateWithStockAndMovementAsync(
                It.Is<InventoryEntry>(e => e.ProductId == 1 && e.Quantity == 5),
                It.Is<Product>(p => p.Stock == 15),
                It.Is<InventoryMovement>(m =>
                    m.MovementType == MovementType.Inbound &&
                    m.Quantity == 5 &&
                    m.ProductId == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_throws_NotFound_when_entry_missing()
    {
        _entryRepository
            .Setup(x => x.GetByIdAsync(8, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryEntry?)null);

        var act = () => _sut.DeleteAsync(8);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_throws_BusinessException_when_stock_insufficient_to_reverse()
    {
        var entry = new InventoryEntry(1, 10) { Id = 8 };
        var product = new Product(1, "Laptop", 4, 100m) { Id = 1 };
        _entryRepository
            .Setup(x => x.GetByIdAsync(8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entry);
        _productRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var act = () => _sut.DeleteAsync(8);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("*stock*");
    }

    [Fact]
    public async Task DeleteAsync_reverses_stock_and_keeps_movement_history()
    {
        var entry = new InventoryEntry(1, 4) { Id = 8 };
        var product = new Product(1, "Laptop", 10, 100m) { Id = 1 };
        _entryRepository
            .Setup(x => x.GetByIdAsync(8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entry);
        _productRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        await _sut.DeleteAsync(8);

        _entryRepository.Verify(
            x => x.DeleteWithStockAsync(
                8,
                It.Is<Product>(p => p.Stock == 6),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _entryRepository.Verify(
            x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
