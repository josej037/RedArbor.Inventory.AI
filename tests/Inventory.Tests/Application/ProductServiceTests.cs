using FluentAssertions;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Products;
using Inventory.Application.Exceptions;
using Inventory.Application.Services.Products;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.Tests.Application;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly Mock<IInventoryEntryRepository> _entryRepository = new();
    private readonly Mock<IInventoryExitRepository> _exitRepository = new();
    private readonly Mock<IInventoryMovementRepository> _movementRepository = new();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _sut = new ProductService(
            _productRepository.Object,
            _categoryRepository.Object,
            _entryRepository.Object,
            _exitRepository.Object,
            _movementRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_throws_NotFound_when_category_missing()
    {
        _categoryRepository
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var act = () => _sut.CreateAsync(new CreateProductRequest
        {
            CategoryId = 99,
            Name = "Laptop",
            Stock = 1,
            UnitPrice = 10m
        });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_adds_product_when_category_exists()
    {
        _categoryRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category("Electronics") { Id = 1 });
        _productRepository
            .Setup(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        var id = await _sut.CreateAsync(new CreateProductRequest
        {
            CategoryId = 1,
            Name = "Laptop",
            Stock = 5,
            UnitPrice = 999m
        });

        id.Should().Be(42);
    }

    [Fact]
    public async Task UpdateAsync_throws_NotFound_when_product_missing()
    {
        _productRepository
            .Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var act = () => _sut.UpdateAsync(5, new UpdateProductRequest
        {
            CategoryId = 1,
            Name = "X",
            Stock = 1,
            UnitPrice = 1m
        });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_throws_BusinessException_when_inventory_history_exists()
    {
        _productRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product(1, "Laptop", 1, 10m) { Id = 1 });
        _entryRepository
            .Setup(x => x.GetByProductIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new InventoryEntry(1, 2) { Id = 9 }]);
        _exitRepository
            .Setup(x => x.GetByProductIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _movementRepository
            .Setup(x => x.GetByProductIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var act = () => _sut.DeleteAsync(1);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("*inventory history*");
    }

    [Fact]
    public async Task DeleteAsync_deletes_when_no_history()
    {
        _productRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product(1, "Laptop", 1, 10m) { Id = 1 });
        _entryRepository
            .Setup(x => x.GetByProductIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _exitRepository
            .Setup(x => x.GetByProductIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _movementRepository
            .Setup(x => x.GetByProductIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await _sut.DeleteAsync(1);

        _productRepository.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
