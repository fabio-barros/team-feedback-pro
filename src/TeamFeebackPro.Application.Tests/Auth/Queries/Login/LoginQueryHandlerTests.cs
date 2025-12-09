using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Auth.Queries.Login;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Auth.Queries.Login;

public class LoginQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly Mock<ILogger<LoginQueryHandler>> _loggerMock;
    private readonly LoginQueryHandler _handler;

    public LoginQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _loggerMock = new Mock<ILogger<LoginQueryHandler>>();

        _handler = new LoginQueryHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenGeneratorMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsSuccessWithToken()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = new User("test@example.com", "hashed_password", "Test User", UserRole.Member, teamId);
        var query = new LoginQuery("test@example.com", "Password123");
        var expectedToken = "jwt_token";

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(query.Password, user.PasswordHash))
            .Returns(true);

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateToken(user.Id, user.Email, user.Role, teamId))
            .Returns(expectedToken);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Token.Should().Be(expectedToken);
        result.Value.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Handle_WithNonExistingEmail_ReturnsFailure()
    {
        // Arrange
        var query = new LoginQuery("nonexistent@example.com", "Password123");

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ReturnsFailure()
    {
        // Arrange
        var user = new User("test@example.com", "hashed_password", "Test User", UserRole.Member, null);
        var query = new LoginQuery("test@example.com", "WrongPassword");

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(query.Password, user.PasswordHash))
            .Returns(false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_CallsJwtTokenGenerator()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = new User("test@example.com", "hashed_password", "Test User", UserRole.Admin, teamId);
        var query = new LoginQuery("test@example.com", "Password123");

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(query.Password, user.PasswordHash))
            .Returns(true);

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateToken(user.Id, user.Email, user.Role, teamId))
            .Returns("token");

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _jwtTokenGeneratorMock.Verify(
            x => x.GenerateToken(user.Id, user.Email, user.Role, teamId),
            Times.Once);
    }
}