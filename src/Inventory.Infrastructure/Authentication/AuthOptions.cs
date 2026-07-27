namespace Inventory.Infrastructure.Authentication;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
