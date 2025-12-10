using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Teams.Commands.DeleteTeam;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Application.Tests.Teams.Commands.DeleteTeam;

public class DeleteTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteTeamCommandHandler>> _loggerMock;
    private readonly DeleteTeamCommandHandler _handler;

    public DeleteTeamCommandHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteTeamCommandHandler>>();

        _handler = new DeleteTeamCommandHandler(
            _teamRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithExistingTeam_DeletesSuccessfully()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team("Engineering", null);
        var command = new DeleteTeamCommand(teamId);

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
        result.Value.Should().BeTrue();

        _teamRepositoryMock.Verify(x => x.DeleteAsync(team, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingTeam_ReturnsFailure()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var command = new DeleteTeamCommand(teamId);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Team?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.TeamNotFound);

        _teamRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CallsDeleteOnRepository()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team("Marketing", null);
        var command = new DeleteTeamCommand(teamId);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _teamRepositoryMock.Verify(x => x.DeleteAsync(team, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CallsSaveChanges()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team("Product", null);
        var command = new DeleteTeamCommand(teamId);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeletesTeamWithManager()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var team = new Team("Engineering", managerId);
        var command = new DeleteTeamCommand(teamId);

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
        _teamRepositoryMock.Verify(x => x.DeleteAsync(team, It.IsAny<CancellationToken>()), Times.Once);
    }
}