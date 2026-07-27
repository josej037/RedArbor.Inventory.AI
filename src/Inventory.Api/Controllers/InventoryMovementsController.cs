using Inventory.Application.DTOs.Inventory;
using Inventory.Application.Services.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

/// <summary>
/// Read-only inventory movement queries. Movements are created as side effects of entries and exits.
/// </summary>
[ApiController]
[Authorize]
[Route("api/inventory/movements")]
public class InventoryMovementsController(IInventoryMovementService inventoryMovementService) : ControllerBase
{
    /// <summary>
    /// Returns all inventory movements.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<InventoryMovementDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<InventoryMovementDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var movements = await inventoryMovementService.GetAllAsync(cancellationToken);
        return Ok(movements);
    }

    /// <summary>
    /// Returns an inventory movement by identifier.
    /// </summary>
    /// <param name="id">Movement identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InventoryMovementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryMovementDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var movement = await inventoryMovementService.GetByIdAsync(id, cancellationToken);
        return movement is null ? NotFound() : Ok(movement);
    }
}
