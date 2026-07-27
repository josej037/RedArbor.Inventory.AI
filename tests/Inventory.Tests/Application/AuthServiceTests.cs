using FluentAssertions;
using Inventory.Application.Abstractions.Authentication;
using Inventory.Application.DTOs.Auth;
using Inventory.Application.Exceptions;
using Inventory.Application.Services.Auth;
using Moq;

namespace Inventory.Tests.Application;

public class AuthServiceTests
{
    private readonly Mock<IAuthConfiguration> _authConfiguration = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGenerator = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _authConfiguration.SetupGet(x => x.Username).Returns("demo");
        _authConfiguration.SetupGet(x => x.Password).Returns("Demo123!");
        _sut = new AuthService(_authConfiguration.Object, _jwtTokenGenerator.Object);
    }

    [Fact]
    public async Task LoginAsync_returns_token_when_credentials_match()
    {
        var expected = new TokenResponse
        {
            AccessToken = "token",
            TokenType = "Bearer",
            ExpiresInMinutes = 60
        };

        _jwtTokenGenerator
            .Setup(x => x.GenerateToken("demo"))
            .Returns(expected);

        var result = await _sut.LoginAsync(new LoginRequest
        {
            Username = "demo",
            Password = "Demo123!"
        });

        result.Should().BeSameAs(expected);
        _jwtTokenGenerator.Verify(x => x.GenerateToken("demo"), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_throws_BusinessException_when_credentials_invalid()
    {
        var act = () => _sut.LoginAsync(new LoginRequest
        {
            Username = "demo",
            Password = "wrong"
        });

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Invalid username or password.");
        _jwtTokenGenerator.Verify(x => x.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_throws_BusinessException_when_username_missing()
    {
        var act = () => _sut.LoginAsync(new LoginRequest
        {
            Username = " ",
            Password = "Demo123!"
        });

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Username and password are required.");
    }
}
