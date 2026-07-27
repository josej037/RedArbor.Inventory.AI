using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Products;
using Inventory.Application.Exceptions;
using Inventory.Domain.Entities;

namespace Inventory.Application.Services.Products;

public class ProductService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IInventoryEntryRepository inventoryEntryRepository,
    IInventoryExitRepository inventoryExitRepository,
    IInventoryMovementRepository inventoryMovementRepository) : IProductService
{
    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : MapToDto(product);
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);
        return products.Select(MapToDto).ToList();
    }

    public async Task<int> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        _ = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Category with id {request.CategoryId} was not found.");

        var product = new Product(
            request.CategoryId,
            request.Name,
            request.Stock,
            request.UnitPrice,
            request.Description);

        return await productRepository.AddAsync(product, cancellationToken);
    }

    public async Task UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product with id {id} was not found.");

        _ = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Category with id {request.CategoryId} was not found.");

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new BusinessException("Product name is required.");
        }

        if (request.Stock < 0)
        {
            throw new BusinessException("Product stock cannot be negative.");
        }

        if (request.UnitPrice < 0)
        {
            throw new BusinessException("Product unit price cannot be negative.");
        }

        product.CategoryId = request.CategoryId;
        product.Name = request.Name.Trim();
        product.Description = request.Description;
        product.Stock = request.Stock;
        product.UnitPrice = request.UnitPrice;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product with id {id} was not found.");

        var entries = await inventoryEntryRepository.GetByProductIdAsync(id, cancellationToken);
        var exits = await inventoryExitRepository.GetByProductIdAsync(id, cancellationToken);
        var movements = await inventoryMovementRepository.GetByProductIdAsync(id, cancellationToken);

        if (entries.Count > 0 || exits.Count > 0 || movements.Count > 0)
        {
            throw new BusinessException("Cannot delete a product that has inventory history.");
        }

        await productRepository.DeleteAsync(id, cancellationToken);
    }

    private static ProductDto MapToDto(Product product) => new()
    {
        Id = product.Id,
        CategoryId = product.CategoryId,
        Name = product.Name,
        Description = product.Description,
        Stock = product.Stock,
        UnitPrice = product.UnitPrice,
        CreatedAtUtc = product.CreatedAtUtc,
        UpdatedAtUtc = product.UpdatedAtUtc
    };
}
