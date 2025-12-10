using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Teams.Queries.GetAllTeams;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Application.Tests.Teams.Queries.GetAllTeams;

public class GetAllTeamsQueryHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<ILogger<GetAllTeamsQueryHandler>> _loggerMock;
    private readonly GetAllTeamsQueryHandler _handler;

    public GetAllTeamsQueryHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _loggerMock = new Mock<ILogger<GetAllTeamsQueryHandler>>();
        _handler = new GetAllTeamsQueryHandler(_teamRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllTeams()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var teams = new List<Team>
        {
            new("Engineering", null),
            new("Marketing", managerId),
            new("Product", null)
        };

        _teamRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        var query = new GetAllTeamsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WithNoTeams_ReturnsEmptyCollection()
    {
        // Arrange
        _teamRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>());

        var query = new GetAllTeamsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MapsTeamsToTeamResults()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var team = new Team("Engineering", managerId);

        _teamRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team> { team });

        var query = new GetAllTeamsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var teamResult = result.Value!.First();
        teamResult.Name.Should().Be("Engineering");
        teamResult.ManagerId.Should().Be(managerId);
    }

    [Fact]
    public async Task Handle_IncludesTeamsWithAndWithoutManagers()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var teams = new List<Team>
        {
            new("Engineering", managerId),
            new("Marketing", null),
            new("Product", Guid.NewGuid())
        };

        _teamRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        var query = new GetAllTeamsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().Contain(t => t.ManagerId.HasValue);
        result.Value.Should().Contain(t => !t.ManagerId.HasValue);
    }

    [Fact]
    public async Task Handle_CallsRepositoryOnce()
    {
        // Arrange
        _teamRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>());

        var query = new GetAllTeamsQuery();

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _teamRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectTeamCount()
    {
        // Arrange
        var teams = new List<Team>
        {
            new("Team 1", null),
            new("Team 2", Guid.NewGuid()),
            new("Team 3", null),
            new("Team 4", Guid.NewGuid()),
            new("Team 5", null)
        };

        _teamRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        var query = new GetAllTeamsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().HaveCount(5);
    }

    [Fact]
    public async Task Handle_MapsAllTeamProperties()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var team = new Team("Data Science", managerId);

        _teamRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team> { team });

        var query = new GetAllTeamsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var teamResult = result.Value!.First();
        teamResult.Id.Should().Be(team.Id);
        teamResult.Name.Should().Be("Data Science");
        teamResult.ManagerId.Should().Be(managerId);
        teamResult.CreatedAt.Should().NotBe(default);
        teamResult.UpdatedAt.Should().NotBe(default);
    }
}