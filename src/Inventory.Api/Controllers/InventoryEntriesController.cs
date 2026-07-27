using Inventory.Application.DTOs.Inventory;
using Inventory.Application.Services.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

/// <summary>
/// Inventory entry operations that increase product stock.
/// </summary>
[ApiController]
[Authorize]
[Route("api/inventory/entries")]
public class InventoryEntriesController(IInventoryEntryService inventoryEntryService) : ControllerBase
{
    /// <summary>
    /// Returns an inventory entry by identifier.
    /// </summary>
    /// <param name="id">Entry identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InventoryEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryEntryDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var entry = await inventoryEntryService.GetByIdAsync(id, cancellationToken);
        return entry is null ? NotFound() : Ok(entry);
    }

    /// <summary>
    /// Returns inventory entries for a product.
    /// </summary>
    /// <param name="productId">Product identifier filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<InventoryEntryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<InventoryEntryDto>>> GetByProductId(
        [FromQuery] int productId,
        CancellationToken cancellationToken)
    {
        var entries = await inventoryEntryService.GetByProductIdAsync(productId, cancellationToken);
        return Ok(entries);
    }

    /// <summary>
    /// Creates an inventory entry and increases product stock.
    /// </summary>
    /// <param name="request">Entry creation payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<int>> Create(
        [FromBody] CreateInventoryEntryRequest request,
        CancellationToken cancellationToken)
    {
        var id = await inventoryEntryService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Deletes an inventory entry and reverses its stock effect.
    /// </summary>
    /// <param name="id">Entry identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await inventoryEntryService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
