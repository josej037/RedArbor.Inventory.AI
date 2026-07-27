using Inventory.Application.Abstractions.Authentication;
using Inventory.Application.DTOs.Auth;
using Inventory.Application.Exceptions;

namespace Inventory.Application.Services.Auth;

public class AuthService(
    IGitHubOAuthClient gitHubOAuthClient,
    IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    public Task<string> GetAuthorizationUrlAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(gitHubOAuthClient.CreateAuthorizationUrl());
    }

    public async Task<TokenResponse> CompleteLoginAsync(
        string code,
        string state,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
        {
            throw new BusinessException("Authorization code and state are required.");
        }

        var githubLogin = await gitHubOAuthClient.AuthenticateAsync(code, state, cancellationToken);
        if (string.IsNullOrWhiteSpace(githubLogin))
        {
            throw new BusinessException("GitHub authentication failed.");
        }

        return jwtTokenGenerator.GenerateToken(githubLogin);
    }
}
