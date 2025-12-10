using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Teams.Commands.CreateTeam;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Teams.Commands.CreateTeam;

public class CreateTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateTeamCommandHandler>> _loggerMock;
    private readonly CreateTeamCommandHandler _handler;

    public CreateTeamCommandHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateTeamCommandHandler>>();

        _handler = new CreateTeamCommandHandler(
            _teamRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesTeamSuccessfully()
    {
        // Arrange
        var command = new CreateTeamCommand("Engineering", null);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Engineering");
        result.Value.ManagerId.Should().BeNull();

        _teamRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithManagerId_ValidatesManagerExists()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var manager = new User("manager@example.com", "hash", "Manager", UserRole.Manager, null);
        var command = new CreateTeamCommand("Engineering", managerId);

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

        _userRepositoryMock.Verify(x => x.GetByIdAsync(managerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingManager_ReturnsFailure()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var command = new CreateTeamCommand("Engineering", managerId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(managerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.UserNotFound);

        _teamRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithoutManagerId_DoesNotValidateManager()
    {
        // Arrange
        var command = new CreateTeamCommand("Product", null);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CallsSaveChanges()
    {
        // Arrange
        var command = new CreateTeamCommand("Marketing", null);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}