using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Inventory.Application.Abstractions.Authentication;
using Inventory.Application.Exceptions;
using Microsoft.Extensions.Options;

namespace Inventory.Infrastructure.Authentication;

public sealed class GitHubOAuthClient(
    HttpClient httpClient,
    IOptions<GitHubOptions> options) : IGitHubOAuthClient
{
    private const string AuthorizeUrl = "https://github.com/login/oauth/authorize";
    private const string TokenUrl = "https://github.com/login/oauth/access_token";
    private const string UserUrl = "https://api.github.com/user";
    private const string Scope = "read:user";
    private static readonly TimeSpan StateTtl = TimeSpan.FromMinutes(10);

    private static readonly ConcurrentDictionary<string, DateTimeOffset> States = new();

    private readonly GitHubOptions _options = options.Value;

    public string CreateAuthorizationUrl()
    {
        EnsureConfigured();

        CleanupExpiredStates();

        var state = Guid.NewGuid().ToString("N");
        States[state] = DateTimeOffset.UtcNow.Add(StateTtl);

        var query =
            $"client_id={Uri.EscapeDataString(_options.ClientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}" +
            $"&scope={Uri.EscapeDataString(Scope)}" +
            $"&state={Uri.EscapeDataString(state)}";

        return $"{AuthorizeUrl}?{query}";
    }

    public async Task<string> AuthenticateAsync(
        string code,
        string state,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();

        CleanupExpiredStates();

        if (!States.TryRemove(state, out var expiresAt) || expiresAt < DateTimeOffset.UtcNow)
        {
            throw new BusinessException("Invalid or expired OAuth state.");
        }

        var accessToken = await ExchangeCodeForAccessTokenAsync(code, cancellationToken);
        return await GetGitHubLoginAsync(accessToken, cancellationToken);
    }

    private async Task<string> ExchangeCodeForAccessTokenAsync(string code, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, TokenUrl);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["code"] = code,
            ["redirect_uri"] = _options.RedirectUri
        });

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new BusinessException("GitHub token exchange failed.");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<GitHubTokenResponse>(cancellationToken);
        if (tokenResponse is null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
        {
            throw new BusinessException("GitHub token exchange failed.");
        }

        return tokenResponse.AccessToken;
    }

    private async Task<string> GetGitHubLoginAsync(string accessToken, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, UserUrl);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.ParseAdd("RedArbor.Inventory.AI");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new BusinessException("GitHub user profile request failed.");
        }

        var user = await response.Content.ReadFromJsonAsync<GitHubUserResponse>(cancellationToken);
        if (user is null || string.IsNullOrWhiteSpace(user.Login))
        {
            throw new BusinessException("GitHub user profile request failed.");
        }

        return user.Login;
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId)
            || string.IsNullOrWhiteSpace(_options.ClientSecret)
            || string.IsNullOrWhiteSpace(_options.RedirectUri))
        {
            throw new BusinessException(
                "GitHub:ClientId, GitHub:ClientSecret, and GitHub:RedirectUri must be configured.");
        }
    }

    private static void CleanupExpiredStates()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var pair in States)
        {
            if (pair.Value < now)
            {
                States.TryRemove(pair.Key, out _);
            }
        }
    }

    private sealed class GitHubTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; init; }
    }

    private sealed class GitHubUserResponse
    {
        [JsonPropertyName("login")]
        public string? Login { get; init; }
    }
}
