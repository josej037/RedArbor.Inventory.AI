using Inventory.Application.DTOs.Auth;
using Inventory.Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

/// <summary>
/// Authentication endpoints for obtaining JWT access tokens.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Authenticates with configured demo credentials and returns a JWT Bearer token.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A JWT access token when credentials are valid.</returns>
    /// <response code="200">Login succeeded.</response>
    /// <response code="400">Invalid credentials or missing fields.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var token = await authService.LoginAsync(request, cancellationToken);
        return Ok(token);
    }
}
