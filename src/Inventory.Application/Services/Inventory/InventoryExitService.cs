using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Inventory;
using Inventory.Application.Exceptions;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;

namespace Inventory.Application.Services.Inventory;

public class InventoryExitService(
    IInventoryExitRepository inventoryExitRepository,
    IProductRepository productRepository) : IInventoryExitService
{
    public async Task<InventoryExitDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var exit = await inventoryExitRepository.GetByIdAsync(id, cancellationToken);
        return exit is null ? null : MapToDto(exit);
    }

    public async Task<IReadOnlyList<InventoryExitDto>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var exits = await inventoryExitRepository.GetByProductIdAsync(productId, cancellationToken);
        return exits.Select(MapToDto).ToList();
    }

    public async Task<int> CreateAsync(
        CreateInventoryExitRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product with id {request.ProductId} was not found.");

        if (product.Stock < request.Quantity)
        {
            throw new BusinessException("Insufficient stock for inventory exit.");
        }

        var exit = new InventoryExit(request.ProductId, request.Quantity, request.Notes);

        product.Stock -= request.Quantity;
        product.UpdatedAtUtc = DateTime.UtcNow;

        var movement = new InventoryMovement(
            request.ProductId,
            MovementType.Outbound,
            request.Quantity,
            request.Notes);

        return await inventoryExitRepository.CreateWithStockAndMovementAsync(
            exit,
            product,
            movement,
            cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var exit = await inventoryExitRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Inventory exit with id {id} was not found.");

        var product = await productRepository.GetByIdAsync(exit.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product with id {exit.ProductId} was not found.");

        product.Stock += exit.Quantity;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await inventoryExitRepository.DeleteWithStockAsync(id, product, cancellationToken);
    }

    private static InventoryExitDto MapToDto(InventoryExit exit) => new()
    {
        Id = exit.Id,
        ProductId = exit.ProductId,
        Quantity = exit.Quantity,
        Notes = exit.Notes,
        CreatedAtUtc = exit.CreatedAtUtc
    };
}
