using Inventory.Application.DTOs.Auth;

namespace Inventory.Application.Services.Auth;

public interface IAuthService
{
    Task<string> GetAuthorizationUrlAsync(CancellationToken cancellationToken = default);

    Task<TokenResponse> CompleteLoginAsync(string code, string state, CancellationToken cancellationToken = default);
}
