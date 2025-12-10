using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Teams.Commands.UpdateTeam;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Teams.Commands.UpdateTeam;

public class UpdateTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateTeamCommandHandler>> _loggerMock;
    private readonly UpdateTeamCommandHandler _handler;

    public UpdateTeamCommandHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateTeamCommandHandler>>();

        _handler = new UpdateTeamCommandHandler(
            _teamRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_UpdatesTeamSuccessfully()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var team = new Team("Old Name", null);
        var manager = new User("manager@example.com", "hash", "Manager", UserRole.Manager, null);

        var command = new UpdateTeamCommand(teamId, "New Name", managerId);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(managerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("New Name");
        result.Value.ManagerId.Should().Be(managerId);

        _teamRepositoryMock.Verify(x => x.UpdateAsync(team, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingTeam_ReturnsFailure()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var command = new UpdateTeamCommand(teamId, "New Name", null);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Team?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.TeamNotFound);

        _teamRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNonExistingManager_ReturnsFailure()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var team = new Team("Engineering", null);
        var command = new UpdateTeamCommand(teamId, "Updated Name", managerId);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(managerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.UserNotFound);

        _teamRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithoutManagerId_UpdatesNameOnly()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team("Old Name", null);
        var command = new UpdateTeamCommand(teamId, "New Name", null);

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
        result.Value!.Name.Should().Be("New Name");
        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdatesTeamName()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team("Old Name", null);
        var command = new UpdateTeamCommand(teamId, "Updated Name", null);

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
        result.Value!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task Handle_AssignsManager()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var team = new Team("Engineering", null);
        var manager = new User("manager@example.com", "hash", "Manager", UserRole.Manager, null);
        var command = new UpdateTeamCommand(teamId, "Engineering", managerId);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(managerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.ManagerId.Should().Be(managerId);
    }
}