using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Categories;
using Inventory.Application.Exceptions;
using Inventory.Domain.Entities;

namespace Inventory.Application.Services.Categories;

public class CategoryService(
    ICategoryRepository categoryRepository,
    IProductRepository productRepository) : ICategoryService
{
    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(id, cancellationToken);
        return category is null ? null : MapToDto(category);
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(MapToDto).ToList();
    }

    public async Task<int> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = new Category(request.Name, request.Description);
        return await categoryRepository.AddAsync(category, cancellationToken);
    }

    public async Task UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Category with id {id} was not found.");

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new BusinessException("Category name is required.");
        }

        category.Name = request.Name.Trim();
        category.Description = request.Description;

        await categoryRepository.UpdateAsync(category, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Category with id {id} was not found.");

        var products = await productRepository.GetByCategoryIdAsync(id, cancellationToken);
        if (products.Count > 0)
        {
            throw new BusinessException("Cannot delete a category that has products.");
        }

        await categoryRepository.DeleteAsync(id, cancellationToken);
    }

    private static CategoryDto MapToDto(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description,
        CreatedAtUtc = category.CreatedAtUtc
    };
}
