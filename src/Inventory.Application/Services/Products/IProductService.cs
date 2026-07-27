using Inventory.Application.DTOs.Products;

namespace Inventory.Application.Services.Products;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);

    Task UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
