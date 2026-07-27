using FluentAssertions;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Inventory;
using Inventory.Application.Exceptions;
using Inventory.Application.Services.Inventory;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Moq;

namespace Inventory.Tests.Application;

public class InventoryExitServiceTests
{
    private readonly Mock<IInventoryExitRepository> _exitRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly InventoryExitService _sut;

    public InventoryExitServiceTests()
    {
        _sut = new InventoryExitService(_exitRepository.Object, _productRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_throws_NotFound_when_product_missing()
    {
        _productRepository
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var act = () => _sut.CreateAsync(new CreateInventoryExitRequest
        {
            ProductId = 99,
            Quantity = 2
        });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_throws_BusinessException_when_insufficient_stock()
    {
        _productRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product(1, "Laptop", 2, 100m) { Id = 1 });

        var act = () => _sut.CreateAsync(new CreateInventoryExitRequest
        {
            ProductId = 1,
            Quantity = 5
        });

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public async Task CreateAsync_decreases_stock_and_creates_outbound_movement()
    {
        var product = new Product(1, "Laptop", 10, 100m) { Id = 1 };
        _productRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _exitRepository
            .Setup(x => x.CreateWithStockAndMovementAsync(
                It.IsAny<InventoryExit>(),
                It.IsAny<Product>(),
                It.IsAny<InventoryMovement>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(77);

        var id = await _sut.CreateAsync(new CreateInventoryExitRequest
        {
            ProductId = 1,
            Quantity = 3,
            Notes = "Sale"
        });

        id.Should().Be(77);
        _exitRepository.Verify(
            x => x.CreateWithStockAndMovementAsync(
                It.Is<InventoryExit>(e => e.ProductId == 1 && e.Quantity == 3),
                It.Is<Product>(p => p.Stock == 7),
                It.Is<InventoryMovement>(m =>
                    m.MovementType == MovementType.Outbound &&
                    m.Quantity == 3 &&
                    m.ProductId == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_throws_NotFound_when_exit_missing()
    {
        _exitRepository
            .Setup(x => x.GetByIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryExit?)null);

        var act = () => _sut.DeleteAsync(4);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_restores_stock_and_keeps_movement_history()
    {
        var exit = new InventoryExit(1, 3) { Id = 4 };
        var product = new Product(1, "Laptop", 7, 100m) { Id = 1 };
        _exitRepository
            .Setup(x => x.GetByIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exit);
        _productRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        await _sut.DeleteAsync(4);

        _exitRepository.Verify(
            x => x.DeleteWithStockAsync(
                4,
                It.Is<Product>(p => p.Stock == 10),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
