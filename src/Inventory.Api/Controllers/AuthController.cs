using Inventory.Application.DTOs.Auth;
using Inventory.Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

/// <summary>
/// Authentication endpoints for GitHub OAuth2 and issuing app JWT access tokens.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Starts GitHub Authorization Code login by redirecting to GitHub.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Redirect to GitHub authorize URL.</returns>
    /// <response code="302">Redirect to GitHub.</response>
    [HttpGet("login")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<IActionResult> Login(CancellationToken cancellationToken)
    {
        var url = await authService.GetAuthorizationUrlAsync(cancellationToken);
        return Redirect(url);
    }

    /// <summary>
    /// Completes GitHub OAuth2 callback and returns an application JWT Bearer token.
    /// </summary>
    /// <param name="code">Authorization code from GitHub.</param>
    /// <param name="state">OAuth state for CSRF protection.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A JWT access token when GitHub authentication succeeds.</returns>
    /// <response code="200">Login succeeded.</response>
    /// <response code="400">Invalid code, state, or GitHub authentication failure.</response>
    [HttpGet("callback")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> Callback(
        [FromQuery] string code,
        [FromQuery] string state,
        CancellationToken cancellationToken)
    {
        var token = await authService.CompleteLoginAsync(code, state, cancellationToken);
        return Ok(token);
    }
}
