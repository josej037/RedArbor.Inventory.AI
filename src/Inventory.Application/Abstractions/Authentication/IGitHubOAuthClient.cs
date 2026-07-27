namespace Inventory.Application.Abstractions.Authentication;

public interface IGitHubOAuthClient
{
    string CreateAuthorizationUrl();

    Task<string> AuthenticateAsync(string code, string state, CancellationToken cancellationToken = default);
}
