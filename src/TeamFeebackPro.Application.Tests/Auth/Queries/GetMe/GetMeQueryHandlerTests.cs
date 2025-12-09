using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Auth.Queries.GetMe;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Auth.Queries.GetMe;

public class GetMeQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<GetMeQueryHandler>> _loggerMock;
    private readonly GetMeQueryHandler _handler;

    public GetMeQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<GetMeQueryHandler>>();
        _handler = new GetMeQueryHandler(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingUser_ReturnsSuccessResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member, teamId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var query = new GetMeQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Email.Should().Be(user.Email);
        result.Value.Name.Should().Be(user.Name);
        result.Value.Role.Should().Be(user.Role);
        result.Value.TeamId.Should().Be(teamId);
    }

    [Fact]
    public async Task Handle_WithNonExistingUser_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var query = new GetMeQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_CallsRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member, null);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var query = new GetMeQuery(userId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_LogsInformation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member, null);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var query = new GetMeQuery(userId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("GetMe requested")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingUser_LogsWarning()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var query = new GetMeQuery(userId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("user not found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}