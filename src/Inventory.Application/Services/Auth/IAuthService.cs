using Inventory.Application.DTOs.Auth;

namespace Inventory.Application.Services.Auth;

public interface IAuthService
{
    Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
