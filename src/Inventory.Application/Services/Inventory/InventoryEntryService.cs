using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Inventory;
using Inventory.Application.Exceptions;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;

namespace Inventory.Application.Services.Inventory;

public class InventoryEntryService(
    IInventoryEntryRepository inventoryEntryRepository,
    IProductRepository productRepository) : IInventoryEntryService
{
    public async Task<InventoryEntryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entry = await inventoryEntryRepository.GetByIdAsync(id, cancellationToken);
        return entry is null ? null : MapToDto(entry);
    }

    public async Task<IReadOnlyList<InventoryEntryDto>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var entries = await inventoryEntryRepository.GetByProductIdAsync(productId, cancellationToken);
        return entries.Select(MapToDto).ToList();
    }

    public async Task<int> CreateAsync(
        CreateInventoryEntryRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product with id {request.ProductId} was not found.");

        var entry = new InventoryEntry(request.ProductId, request.Quantity, request.Notes);

        product.Stock += request.Quantity;
        product.UpdatedAtUtc = DateTime.UtcNow;

        var movement = new InventoryMovement(
            request.ProductId,
            MovementType.Inbound,
            request.Quantity,
            request.Notes);

        return await inventoryEntryRepository.CreateWithStockAndMovementAsync(
            entry,
            product,
            movement,
            cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entry = await inventoryEntryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Inventory entry with id {id} was not found.");

        var product = await productRepository.GetByIdAsync(entry.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product with id {entry.ProductId} was not found.");

        if (product.Stock < entry.Quantity)
        {
            throw new BusinessException("Cannot reverse inventory entry because product stock would become negative.");
        }

        product.Stock -= entry.Quantity;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await inventoryEntryRepository.DeleteWithStockAsync(id, product, cancellationToken);
    }

    private static InventoryEntryDto MapToDto(InventoryEntry entry) => new()
    {
        Id = entry.Id,
        ProductId = entry.ProductId,
        Quantity = entry.Quantity,
        Notes = entry.Notes,
        CreatedAtUtc = entry.CreatedAtUtc
    };
}
