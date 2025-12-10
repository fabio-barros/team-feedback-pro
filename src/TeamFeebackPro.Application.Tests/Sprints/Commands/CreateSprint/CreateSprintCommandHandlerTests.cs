using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Sprints.Commands.CreateSprint;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Sprints.Commands.CreateSprint;

public class CreateSprintCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ISprintRepository> _sprintRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateSprintCommandHandler>> _loggerMock;
    private readonly CreateSprintCommandHandler _handler;

    public CreateSprintCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _sprintRepositoryMock = new Mock<ISprintRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateSprintCommandHandler>>();

        _handler = new CreateSprintCommandHandler(
            _userRepositoryMock.Object,
            _sprintRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesSprintSuccessfully()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var manager = new User("manager@example.com", "hash", "Manager", UserRole.Manager, teamId);

        var command = new CreateSprintCommand(
            managerId,
            "Sprint 1",
            "First sprint of the project with initial features.",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14)
        );

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(managerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);

        _sprintRepositoryMock
            .Setup(x => x.ExistAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                teamId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Sprint 1");
        result.Value.TeamId.Should().Be(teamId);

        _sprintRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sprint>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingManager_ReturnsFailure()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var command = new CreateSprintCommand(
            managerId,
            "Sprint 1",
            "Description with sufficient length for validation.",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14)
        );

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(managerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.UserNotFound);

        _sprintRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sprint>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithManagerWithoutTeam_ReturnsFailure()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var manager = new User("manager@example.com", "hash", "Manager", UserRole.Manager, null);

        var command = new CreateSprintCommand(
            managerId,
            "Sprint 1",
            "Description with sufficient length for validation.",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14)
        );

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(managerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.TeamNotFound);

        _sprintRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sprint>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithOverlappingSprint_ReturnsFailure()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var manager = new User("manager@example.com", "hash", "Manager", UserRole.Manager, teamId);

        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(14);

        var command = new CreateSprintCommand(
            managerId,
            "Sprint 2",
            "Another sprint that overlaps with existing one.",
            startDate,
            endDate
        );

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(managerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);

        _sprintRepositoryMock
            .Setup(x => x.ExistAsync(
                startDate.Date,
                endDate.Date,
                teamId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.SprintAlreadyExistsByDate);

        _sprintRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sprint>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidatesSprintDatesAreChecked()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var manager = new User("manager@example.com", "hash", "Manager", UserRole.Manager, teamId);

        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 1, 15);

        var command = new CreateSprintCommand(
            managerId,
            "Sprint 1",
            "Testing date validation for sprint creation.",
            startDate,
            endDate
        );

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(managerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);

        _sprintRepositoryMock
            .Setup(x => x.ExistAsync(
                startDate.Date,
                endDate.Date,
                teamId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _sprintRepositoryMock.Verify(
            x => x.ExistAsync(startDate.Date, endDate.Date, teamId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_CreatesSprintWithCorrectProperties()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var manager = new User("manager@example.com", "hash", "Manager", UserRole.Manager, teamId);

        var command = new CreateSprintCommand(
            managerId,
            "Q1 Sprint",
            "First quarter sprint with major feature releases.",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(21)
        );

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(managerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);

        _sprintRepositoryMock
            .Setup(x => x.ExistAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                teamId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Value!.Name.Should().Be("Q1 Sprint");
        result.Value.Description.Should().Contain("First quarter");
        result.Value.TeamId.Should().Be(teamId);
    }

    [Fact]
    public async Task Handle_CallsSaveChanges()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var manager = new User("manager@example.com", "hash", "Manager", UserRole.Manager, teamId);

        var command = new CreateSprintCommand(
            managerId,
            "Sprint 1",
            "Description with sufficient length for validation.",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14)
        );

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(managerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);

        _sprintRepositoryMock
            .Setup(x => x.ExistAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                teamId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}