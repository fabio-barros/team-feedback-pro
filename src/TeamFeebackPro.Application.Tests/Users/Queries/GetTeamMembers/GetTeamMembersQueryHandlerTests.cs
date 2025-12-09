using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Users.Queries.GetTeamMembers;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;
using System.Reflection;

namespace TeamFeedbackPro.Application.Tests.Users.Queries.GetTeamMembers;

public class GetTeamMembersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<GetTeamMembersQueryHandler>> _loggerMock;
    private readonly GetTeamMembersQueryHandler _handler;

    public GetTeamMembersQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<GetTeamMembersQueryHandler>>();
        _handler = new GetTeamMembersQueryHandler(_userRepositoryMock.Object, _loggerMock.Object);
    }

    private static void SetUserId(User user, Guid id)
    {
        // Use reflection to set the Id property
        var property = typeof(User).GetProperty("Id");
        if (property != null && property.CanWrite)
        {
            property.SetValue(user, id);
        }
        else
        {
            var field = typeof(User).BaseType?.GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(user, id);
        }
    }

    [Fact]
    public async Task Handle_WithUserInTeam_ReturnsTeamMembers()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var currentUser = new User("current@example.com", "hash", "Current User", UserRole.Member, teamId);
        SetUserId(currentUser, userId);

        var member1 = new User("member1@example.com", "hash", "Member One", UserRole.Member, teamId);
        var member2 = new User("member2@example.com", "hash", "Member Two", UserRole.Manager, teamId);

        var teamMembers = new List<User> { currentUser, member1, member2 };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);

        _userRepositoryMock
            .Setup(x => x.GetByTeamIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teamMembers);

        var query = new GetTeamMembersQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2); // Excludes current user
        result.Value.Should().NotContain(m => m.Email == "current@example.com");
        result.Value.Should().Contain(m => m.Email == "member1@example.com");
        result.Value.Should().Contain(m => m.Email == "member2@example.com");
    }

    [Fact]
    public async Task Handle_WithNonExistingUser_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var query = new GetTeamMembersQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_WithUserWithoutTeam_ReturnsEmptyCollection()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUser = new User("current@example.com", "hash", "Current User", UserRole.Admin, null);
        SetUserId(currentUser, userId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);

        var query = new GetTeamMembersQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        _userRepositoryMock.Verify(x => x.GetByTeamIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ExcludesCurrentUserFromResults()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var currentUser = new User("current@example.com", "hash", "Current User", UserRole.Member, teamId);
        SetUserId(currentUser, userId);

        var otherUser = new User("other@example.com", "hash", "Other User", UserRole.Member, teamId);

        var teamMembers = new List<User> { currentUser, otherUser };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);

        _userRepositoryMock
            .Setup(x => x.GetByTeamIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teamMembers);

        var query = new GetTeamMembersQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().HaveCount(1);
        result.Value.Should().Contain(m => m.Email == "other@example.com");
        result.Value.Should().NotContain(m => m.Email == "current@example.com");
    }

    [Fact]
    public async Task Handle_MapsTeamMembersToResults()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var currentUser = new User("current@example.com", "hash", "Current User", UserRole.Member, teamId);
        SetUserId(currentUser, userId);

        var otherUser = new User("other@example.com", "hash", "Other User", UserRole.Manager, teamId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);

        _userRepositoryMock
            .Setup(x => x.GetByTeamIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { currentUser, otherUser });

        var query = new GetTeamMembersQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var member = result.Value!.First();
        member.Id.Should().Be(otherUser.Id);
        member.Name.Should().Be("Other User");
        member.Email.Should().Be("other@example.com");
        member.Role.Should().Be(UserRole.Manager.ToString());
    }

    [Fact]
    public async Task Handle_WithOnlyCurrentUserInTeam_ReturnsEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var currentUser = new User("current@example.com", "hash", "Current User", UserRole.Member, teamId);
        SetUserId(currentUser, userId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);

        _userRepositoryMock
            .Setup(x => x.GetByTeamIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { currentUser });

        var query = new GetTeamMembersQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CallsRepositories()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var currentUser = new User("current@example.com", "hash", "Current User", UserRole.Member, teamId);
        SetUserId(currentUser, userId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);

        _userRepositoryMock
            .Setup(x => x.GetByTeamIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { currentUser });

        var query = new GetTeamMembersQuery(userId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(x => x.GetByTeamIdAsync(teamId, It.IsAny<CancellationToken>()), Times.Once);
    }
}