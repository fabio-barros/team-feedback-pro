using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;
using TeamFeedBackPro.Infrastructure.Persistence;
using TeamFeedBackPro.Infrastructure.Persistence.Repositories;

namespace TeamFeebackPro.Infrastructure.Tests.Persistence.Repositories;

public class TeamRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly TeamRepository _repository;

    public TeamRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new TeamRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingTeam_ReturnsTeam()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(team.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(team.Id);
        result.Name.Should().Be("Engineering");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingTeam_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithMembers_IncludesMembersNavigation()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var user1 = new User("user1@example.com", "hash", "User 1", UserRole.Member, team.Id);
        var user2 = new User("user2@example.com", "hash", "User 2", UserRole.Member, team.Id);
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(team.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Members.Should().HaveCount(2);
        result.Members.Should().Contain(u => u.Email == "user1@example.com");
        result.Members.Should().Contain(u => u.Email == "user2@example.com");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTeams()
    {
        // Arrange
        var team1 = new Team("Engineering");
        var team2 = new Team("Product");
        _context.Teams.AddRange(team1, team2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Name == "Engineering");
        result.Should().Contain(t => t.Name == "Product");
    }

    [Fact]
    public async Task GetAllAsync_WithNoTeams_ReturnsEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_IncludesMembersForAllTeams()
    {
        // Arrange
        var team1 = new Team("Engineering");
        var team2 = new Team("Product");
        _context.Teams.AddRange(team1, team2);
        await _context.SaveChangesAsync();

        var user1 = new User("user1@example.com", "hash", "User 1", UserRole.Member, team1.Id);
        var user2 = new User("user2@example.com", "hash", "User 2", UserRole.Member, team2.Id);
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        var engineeringTeam = result.First(t => t.Name == "Engineering");
        var productTeam = result.First(t => t.Name == "Product");

        engineeringTeam.Members.Should().HaveCount(1);
        engineeringTeam.Members.Should().Contain(u => u.Email == "user1@example.com");

        productTeam.Members.Should().HaveCount(1);
        productTeam.Members.Should().Contain(u => u.Email == "user2@example.com");
    }

    [Fact]
    public async Task AddAsync_AddsTeamToContext()
    {
        // Arrange
        var team = new Team("Engineering");

        // Act
        var result = await _repository.AddAsync(team);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().Be(team);
        var savedTeam = await _context.Teams.FirstOrDefaultAsync(t => t.Id == team.Id);
        savedTeam.Should().NotBeNull();
        savedTeam!.Name.Should().Be("Engineering");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTeam()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Act
        team.UpdateName("Engineering - Updated");
        await _repository.UpdateAsync(team);
        await _context.SaveChangesAsync();

        // Assert
        var updatedTeam = await _context.Teams.FirstOrDefaultAsync(t => t.Id == team.Id);
        updatedTeam.Should().NotBeNull();
        updatedTeam!.Name.Should().Be("Engineering - Updated");
    }

    [Fact]
    public async Task DeleteAsync_RemovesTeam()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(team);
        await _context.SaveChangesAsync();

        // Assert
        var deletedTeam = await _context.Teams.FirstOrDefaultAsync(t => t.Id == team.Id);
        deletedTeam.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WithManagerId_SetsManagerId()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var team = new Team("Engineering", managerId);

        // Act
        await _repository.AddAsync(team);
        await _context.SaveChangesAsync();

        // Assert
        var savedTeam = await _context.Teams.FirstOrDefaultAsync(t => t.Id == team.Id);
        savedTeam.Should().NotBeNull();
        savedTeam!.ManagerId.Should().Be(managerId);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}