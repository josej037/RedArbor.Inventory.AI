namespace Inventory.Application.Abstractions.Authentication;

public interface IAuthConfiguration
{
    string Username { get; }

    string Password { get; }
}
