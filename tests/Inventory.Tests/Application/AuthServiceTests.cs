using FluentAssertions;
using Inventory.Application.Abstractions.Authentication;
using Inventory.Application.DTOs.Auth;
using Inventory.Application.Exceptions;
using Inventory.Application.Services.Auth;
using Moq;

namespace Inventory.Tests.Application;

public class AuthServiceTests
{
    private readonly Mock<IGitHubOAuthClient> _gitHubOAuthClient = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGenerator = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_gitHubOAuthClient.Object, _jwtTokenGenerator.Object);
    }

    [Fact]
    public async Task GetAuthorizationUrlAsync_returns_url_from_github_client()
    {
        _gitHubOAuthClient
            .Setup(x => x.CreateAuthorizationUrl())
            .Returns("https://github.com/login/oauth/authorize?client_id=abc");

        var result = await _sut.GetAuthorizationUrlAsync();

        result.Should().Be("https://github.com/login/oauth/authorize?client_id=abc");
    }

    [Fact]
    public async Task CompleteLoginAsync_returns_token_when_github_authentication_succeeds()
    {
        var expected = new TokenResponse
        {
            AccessToken = "token",
            TokenType = "Bearer",
            ExpiresInMinutes = 60
        };

        _gitHubOAuthClient
            .Setup(x => x.AuthenticateAsync("valid-code", "valid-state", It.IsAny<CancellationToken>()))
            .ReturnsAsync("octocat");
        _jwtTokenGenerator
            .Setup(x => x.GenerateToken("octocat"))
            .Returns(expected);

        var result = await _sut.CompleteLoginAsync("valid-code", "valid-state");

        result.Should().BeSameAs(expected);
        _jwtTokenGenerator.Verify(x => x.GenerateToken("octocat"), Times.Once);
    }

    [Fact]
    public async Task CompleteLoginAsync_throws_BusinessException_when_code_or_state_missing()
    {
        var act = () => _sut.CompleteLoginAsync(" ", "state");

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Authorization code and state are required.");
        _gitHubOAuthClient.Verify(
            x => x.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CompleteLoginAsync_throws_BusinessException_when_github_authentication_fails()
    {
        _gitHubOAuthClient
            .Setup(x => x.AuthenticateAsync("bad-code", "bad-state", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BusinessException("Invalid OAuth state."));

        var act = () => _sut.CompleteLoginAsync("bad-code", "bad-state");

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Invalid OAuth state.");
        _jwtTokenGenerator.Verify(x => x.GenerateToken(It.IsAny<string>()), Times.Never);
    }
}
