using Inventory.Application.DTOs.Categories;

namespace Inventory.Application.Services.Categories;

public interface ICategoryService
{
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);

    Task UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
