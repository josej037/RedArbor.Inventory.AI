using Inventory.Application.Abstractions.Authentication;
using Microsoft.Extensions.Options;

namespace Inventory.Infrastructure.Authentication;

public sealed class AuthConfiguration(IOptions<AuthOptions> options) : IAuthConfiguration
{
    private readonly AuthOptions _options = options.Value;

    public string Username => _options.Username;

    public string Password => _options.Password;
}
