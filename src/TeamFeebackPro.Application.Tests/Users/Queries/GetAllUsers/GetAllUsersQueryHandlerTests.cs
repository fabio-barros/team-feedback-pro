using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Users.Queries.GetAllUsers;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<GetAllUsersQueryHandler>> _loggerMock;
    private readonly GetAllUsersQueryHandler _handler;

    public GetAllUsersQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<GetAllUsersQueryHandler>>();
        _handler = new GetAllUsersQueryHandler(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllUsers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var users = new List<User>
        {
            new("user1@example.com", "hash1", "User One", UserRole.Admin, null),
            new("user2@example.com", "hash2", "User Two", UserRole.Manager, teamId),
            new("user3@example.com", "hash3", "User Three", UserRole.Member, teamId)
        };

        _userRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var query = new GetAllUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WithNoUsers_ReturnsEmptyCollection()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        var query = new GetAllUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MapsUsersToUserResults()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", "Test User", UserRole.Manager, teamId);

        _userRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { user });

        var query = new GetAllUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var userResult = result.Value!.First();
        userResult.Email.Should().Be("test@example.com");
        userResult.Name.Should().Be("Test User");
        userResult.Role.Should().Be(UserRole.Manager.ToString());
        userResult.TeamId.Should().Be(teamId);
    }

    [Fact]
    public async Task Handle_IncludesUsersWithDifferentRoles()
    {
        // Arrange
        var users = new List<User>
        {
            new("admin@example.com", "hash", "Admin", UserRole.Admin, null),
            new("manager@example.com", "hash", "Manager", UserRole.Manager, Guid.NewGuid()),
            new("member@example.com", "hash", "Member", UserRole.Member, Guid.NewGuid())
        };

        _userRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var query = new GetAllUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().Contain(u => u.Role == UserRole.Admin.ToString());
        result.Value.Should().Contain(u => u.Role == UserRole.Manager.ToString());
        result.Value.Should().Contain(u => u.Role == UserRole.Member.ToString());
    }

    [Fact]
    public async Task Handle_CallsRepositoryOnce()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        var query = new GetAllUsersQuery();

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}