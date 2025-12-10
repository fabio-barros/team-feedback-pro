using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Teams.Queries.GetTeam;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Application.Tests.Teams.Queries.GetTeam;

public class GetTeamQueryHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<ILogger<GetTeamQueryHandler>> _loggerMock;
    private readonly GetTeamQueryHandler _handler;

    public GetTeamQueryHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _loggerMock = new Mock<ILogger<GetTeamQueryHandler>>();
        _handler = new GetTeamQueryHandler(_teamRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingTeam_ReturnsTeamResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var team = new Team("Engineering", managerId);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var query = new GetTeamQuery(teamId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(team.Id);
        result.Value.Name.Should().Be("Engineering");
        result.Value.ManagerId.Should().Be(managerId);
    }

    [Fact]
    public async Task Handle_WithNonExistingTeam_ReturnsFailure()
    {
        // Arrange
        var teamId = Guid.NewGuid();

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Team?)null);

        var query = new GetTeamQuery(teamId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.TeamNotFound);
    }

    [Fact]
    public async Task Handle_WithTeamWithoutManager_ReturnsTeamWithNullManagerId()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team("Marketing", null);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var query = new GetTeamQuery(teamId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.ManagerId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_CallsRepositoryOnce()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team("Product", null);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var query = new GetTeamQuery(teamId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _teamRepositoryMock.Verify(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MapsAllTeamProperties()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var team = new Team("Data Science", managerId);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var query = new GetTeamQuery(teamId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value!.Id.Should().Be(team.Id);
        result.Value.Name.Should().Be("Data Science");
        result.Value.ManagerId.Should().Be(managerId);
        result.Value.CreatedAt.Should().NotBe(default);
        result.Value.UpdatedAt.Should().NotBe(default);
    }

    [Fact]
    public async Task Handle_WithDifferentTeamNames_ReturnsCorrectTeam()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team("Customer Support", null);

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var query = new GetTeamQuery(teamId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Customer Support");
    }

    [Fact]
    public async Task Handle_ReturnsTeamWithCreatedAndUpdatedDates()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team("Operations", Guid.NewGuid());

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var query = new GetTeamQuery(teamId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value!.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Value.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}