using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Users.Queries.GetUser;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Users.Queries.GetUser;

public class GetUserQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<GetUserQueryHandler>> _loggerMock;
    private readonly GetUserQueryHandler _handler;

    public GetUserQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<GetUserQueryHandler>>();
        _handler = new GetUserQueryHandler(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingUser_ReturnsUserResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member, teamId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var query = new GetUserQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(user.Id);
        result.Value.Email.Should().Be("test@example.com");
        result.Value.Name.Should().Be("Test User");
        result.Value.Role.Should().Be(UserRole.Member.ToString());
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

        var query = new GetUserQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_WithUserWithoutTeam_ReturnsUserWithNullTeamId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", "Test User", UserRole.Admin, null);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var query = new GetUserQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.TeamId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_CallsRepositoryOnce()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member, null);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var query = new GetUserQuery(userId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MapsAllUserProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var user = new User("manager@example.com", "hash", "Manager User", UserRole.Manager, teamId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var query = new GetUserQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value!.Email.Should().Be("manager@example.com");
        result.Value.Name.Should().Be("Manager User");
        result.Value.Role.Should().Be(UserRole.Manager.ToString());
        result.Value.TeamId.Should().Be(teamId);
        result.Value.CreatedAt.Should().NotBe(default);
        result.Value.UpdatedAt.Should().NotBe(default);
    }
}