namespace Inventory.Application.DTOs.Auth;

public sealed class TokenResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";
    public int ExpiresInMinutes { get; init; }
}
