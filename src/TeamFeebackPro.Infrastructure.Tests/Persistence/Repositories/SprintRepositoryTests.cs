using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedBackPro.Infrastructure.Persistence;
using TeamFeedBackPro.Infrastructure.Persistence.Repositories;

namespace TeamFeebackPro.Infrastructure.Tests.Persistence.Repositories;

public class SprintRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly SprintRepository _repository;

    public SprintRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new SprintRepository(_context);
    }

    [Fact]
    public async Task GetActualSprintAsync_WithActiveSprint_ReturnsSprint()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var activeSprint = new Sprint(
            "Sprint 1",
            "Current sprint",
            DateTime.Now.AddDays(-3),
            DateTime.Now.AddDays(11),
            team.Id
        );
        _context.Sprints.Add(activeSprint);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActualSprintAsync(team.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(activeSprint.Id);
        result.Name.Should().Be("Sprint 1");
    }

    [Fact]
    public async Task GetActualSprintAsync_WithNoActiveSprint_ReturnsNull()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var pastSprint = new Sprint(
            "Sprint 1",
            "Past sprint",
            DateTime.Now.AddDays(-20),
            DateTime.Now.AddDays(-6),
            team.Id
        );
        _context.Sprints.Add(pastSprint);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActualSprintAsync(team.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetActualSprintAsync_WithFutureSprint_ReturnsNull()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var futureSprint = new Sprint(
            "Sprint 2",
            "Future sprint",
            DateTime.Now.AddDays(5),
            DateTime.Now.AddDays(19),
            team.Id
        );
        _context.Sprints.Add(futureSprint);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActualSprintAsync(team.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetActualSprintAsync_WithMultipleSprints_ReturnsOnlyActiveSprint()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var pastSprint = new Sprint("Sprint 1", "Past", DateTime.Now.AddDays(-30), DateTime.Now.AddDays(-16), team.Id);
        var activeSprint = new Sprint("Sprint 2", "Active", DateTime.Now.AddDays(-3), DateTime.Now.AddDays(11), team.Id);
        var futureSprint = new Sprint("Sprint 3", "Future", DateTime.Now.AddDays(15), DateTime.Now.AddDays(29), team.Id);

        _context.Sprints.AddRange(pastSprint, activeSprint, futureSprint);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActualSprintAsync(team.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Sprint 2");
    }

    [Fact]
    public async Task AddAsync_AddsSprint()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var sprint = new Sprint(
            "Sprint 1",
            "New sprint",
            DateTime.Now,
            DateTime.Now.AddDays(14),
            team.Id
        );

        // Act
        var result = await _repository.AddAsync(sprint);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().Be(sprint);
        var savedSprint = await _context.Sprints.FirstOrDefaultAsync(s => s.Id == sprint.Id);
        savedSprint.Should().NotBeNull();
        savedSprint!.Name.Should().Be("Sprint 1");
    }

    [Fact]
    public async Task ExistAsync_WithOverlappingSprintAtStart_ReturnsTrue()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var existingSprint = new Sprint(
            "Sprint 1",
            "Existing",
            new DateTime(2025, 1, 1),
            new DateTime(2025, 1, 14),
            team.Id
        );
        _context.Sprints.Add(existingSprint);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistAsync(
            new DateTime(2024, 12, 28),
            new DateTime(2025, 1, 7),
            team.Id
        );

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistAsync_WithOverlappingSprintAtEnd_ReturnsTrue()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var existingSprint = new Sprint(
            "Sprint 1",
            "Existing",
            new DateTime(2025, 1, 1),
            new DateTime(2025, 1, 14),
            team.Id
        );
        _context.Sprints.Add(existingSprint);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistAsync(
            new DateTime(2025, 1, 10),
            new DateTime(2025, 1, 20),
            team.Id
        );

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistAsync_WithNoOverlap_ReturnsFalse()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var existingSprint = new Sprint(
            "Sprint 1",
            "Existing",
            new DateTime(2025, 1, 1),
            new DateTime(2025, 1, 14),
            team.Id
        );
        _context.Sprints.Add(existingSprint);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistAsync(
            new DateTime(2025, 1, 15),
            new DateTime(2025, 1, 28),
            team.Id
        );

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistAsync_WithDifferentTeam_ReturnsFalse()
    {
        // Arrange
        var team1 = new Team("Engineering");
        var team2 = new Team("Marketing");
        _context.Teams.AddRange(team1, team2);
        await _context.SaveChangesAsync();

        var existingSprint = new Sprint(
            "Sprint 1",
            "Existing",
            new DateTime(2025, 1, 1),
            new DateTime(2025, 1, 14),
            team1.Id
        );
        _context.Sprints.Add(existingSprint);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistAsync(
            new DateTime(2025, 1, 5),
            new DateTime(2025, 1, 10),
            team2.Id
        );

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}