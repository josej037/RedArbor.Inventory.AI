using FluentAssertions;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Categories;
using Inventory.Application.Exceptions;
using Inventory.Application.Services.Categories;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.Tests.Application;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _sut = new CategoryService(_categoryRepository.Object, _productRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_returns_dto_when_found()
    {
        var category = new Category("Electronics") { Id = 1 };
        _categoryRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Electronics");
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_when_missing()
    {
        _categoryRepository
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var result = await _sut.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_adds_category_and_returns_id()
    {
        _categoryRepository
            .Setup(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        var id = await _sut.CreateAsync(new CreateCategoryRequest { Name = "Tools", Description = "Hand tools" });

        id.Should().Be(10);
        _categoryRepository.Verify(
            x => x.AddAsync(It.Is<Category>(c => c.Name == "Tools" && c.Description == "Hand tools"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_throws_NotFound_when_missing()
    {
        _categoryRepository
            .Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var act = () => _sut.UpdateAsync(5, new UpdateCategoryRequest { Name = "Updated" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_updates_existing_category()
    {
        var category = new Category("Old") { Id = 2 };
        _categoryRepository
            .Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        await _sut.UpdateAsync(2, new UpdateCategoryRequest { Name = "New", Description = "Desc" });

        _categoryRepository.Verify(
            x => x.UpdateAsync(It.Is<Category>(c => c.Id == 2 && c.Name == "New" && c.Description == "Desc"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_throws_NotFound_when_missing()
    {
        _categoryRepository
            .Setup(x => x.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var act = () => _sut.DeleteAsync(3);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_throws_BusinessException_when_products_exist()
    {
        _categoryRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category("Electronics") { Id = 1 });
        _productRepository
            .Setup(x => x.GetByCategoryIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Product(1, "Laptop", 1, 10m) { Id = 7 }]);

        var act = () => _sut.DeleteAsync(1);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("*products*");
    }

    [Fact]
    public async Task DeleteAsync_deletes_when_no_products()
    {
        _categoryRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category("Empty") { Id = 1 });
        _productRepository
            .Setup(x => x.GetByCategoryIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await _sut.DeleteAsync(1);

        _categoryRepository.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
