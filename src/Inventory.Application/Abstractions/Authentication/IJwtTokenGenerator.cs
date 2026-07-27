using Inventory.Application.DTOs.Auth;

namespace Inventory.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    TokenResponse GenerateToken(string username);
}
