using Inventory.Application.DTOs.Inventory;
using Inventory.Application.Services.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

/// <summary>
/// Inventory exit operations that decrease product stock.
/// </summary>
[ApiController]
[Authorize]
[Route("api/inventory/exits")]
public class InventoryExitsController(IInventoryExitService inventoryExitService) : ControllerBase
{
    /// <summary>
    /// Returns an inventory exit by identifier.
    /// </summary>
    /// <param name="id">Exit identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InventoryExitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryExitDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var exit = await inventoryExitService.GetByIdAsync(id, cancellationToken);
        return exit is null ? NotFound() : Ok(exit);
    }

    /// <summary>
    /// Returns inventory exits for a product.
    /// </summary>
    /// <param name="productId">Product identifier filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<InventoryExitDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<InventoryExitDto>>> GetByProductId(
        [FromQuery] int productId,
        CancellationToken cancellationToken)
    {
        var exits = await inventoryExitService.GetByProductIdAsync(productId, cancellationToken);
        return Ok(exits);
    }

    /// <summary>
    /// Creates an inventory exit and decreases product stock.
    /// </summary>
    /// <param name="request">Exit creation payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<int>> Create(
        [FromBody] CreateInventoryExitRequest request,
        CancellationToken cancellationToken)
    {
        var id = await inventoryExitService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Deletes an inventory exit and reverses its stock effect.
    /// </summary>
    /// <param name="id">Exit identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await inventoryExitService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
