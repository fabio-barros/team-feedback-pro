using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Users.Commands.UpdateUser;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Users.Commands.UpdateUser;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateUserCommandHandler>> _loggerMock;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateUserCommandHandler>>();

        _handler = new UpdateUserCommandHandler(
            _userRepositoryMock.Object,
            _teamRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_UpdatesUserSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member, null);
        var team = new Team("Engineering", null);

        var command = new UpdateUserCommand(userId, "Updated Name", UserRole.Manager, teamId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Role.Should().Be(UserRole.Manager.ToString());
        result.Value.TeamId.Should().Be(teamId);

        _userRepositoryMock.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingUser_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand(userId, "Updated Name", UserRole.Manager, null);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.UserNotFound);

        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNonExistingTeam_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member, null);

        var command = new UpdateUserCommand(userId, "Updated Name", UserRole.Manager, teamId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Team?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.TeamNotFound);

        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithoutTeamId_UpdatesRoleOnly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member, null);

        var command = new UpdateUserCommand(userId, "Updated Name", UserRole.Admin, null);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Role.Should().Be(UserRole.Admin.ToString());
        result.Value.TeamId.Should().BeNull();

        _teamRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdatesRole()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member, teamId);
        var team = new Team("Engineering", null);

        var command = new UpdateUserCommand(userId, "Test User", UserRole.Manager, teamId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Role.Should().Be(UserRole.Manager.ToString());
    }

    [Fact]
    public async Task Handle_AssignsTeam()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newTeamId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member, null);
        var team = new Team("New Team", null);

        var command = new UpdateUserCommand(userId, "Test User", UserRole.Member, newTeamId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(newTeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.TeamId.Should().Be(newTeamId);
    }
}