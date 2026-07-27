using Inventory.Application.Abstractions.Authentication;
using Inventory.Application.DTOs.Auth;
using Inventory.Application.Exceptions;

namespace Inventory.Application.Services.Auth;

public class AuthService(
    IAuthConfiguration authConfiguration,
    IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    public Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new BusinessException("Username and password are required.");
        }

        var usernameMatches = string.Equals(
            request.Username.Trim(),
            authConfiguration.Username,
            StringComparison.Ordinal);

        var passwordMatches = string.Equals(
            request.Password,
            authConfiguration.Password,
            StringComparison.Ordinal);

        if (!usernameMatches || !passwordMatches)
        {
            throw new BusinessException("Invalid username or password.");
        }

        var token = jwtTokenGenerator.GenerateToken(authConfiguration.Username);
        return Task.FromResult(token);
    }
}
