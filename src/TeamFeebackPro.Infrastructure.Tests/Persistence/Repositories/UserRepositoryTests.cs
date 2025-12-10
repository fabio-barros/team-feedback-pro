using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;
using TeamFeedBackPro.Infrastructure.Persistence;
using TeamFeedBackPro.Infrastructure.Persistence.Repositories;

namespace TeamFeedbackPro.Infrastructure.Tests.Persistence.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingUser_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithTeam_IncludesTeamNavigation()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var user = new User("test@example.com", "hash", "Test User", UserRole.Member, team.Id);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Team.Should().NotBeNull();
        result.Team!.Name.Should().Be("Engineering");
    }

    [Fact]
    public async Task GetByEmailAsync_WithExistingEmail_ReturnsUser()
    {
        // Arrange
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("test@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_WithUpperCaseEmail_ReturnsUser()
    {
        // Arrange
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("TEST@EXAMPLE.COM");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_WithNonExistingEmail_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsByEmailAsync_WithExistingEmail_ReturnsTrue()
    {
        // Arrange
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByEmailAsync("test@example.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_WithUpperCaseEmail_ReturnsTrue()
    {
        // Arrange
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByEmailAsync("TEST@EXAMPLE.COM");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_WithNonExistingEmail_ReturnsFalse()
    {
        // Act
        var result = await _repository.ExistsByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        var user1 = new User("user1@example.com", "hash", "User 1", UserRole.Member);
        var user2 = new User("user2@example.com", "hash", "User 2", UserRole.Manager);
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Email == "user1@example.com");
        result.Should().Contain(u => u.Email == "user2@example.com");
    }

    [Fact]
    public async Task GetAllAsync_WithNoUsers_ReturnsEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddAsync_AddsUserToContext()
    {
        // Arrange
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member);

        // Act
        var result = await _repository.AddAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().Be(user);
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser()
    {
        // Arrange
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        user.UpdateName("Updated Name");
        await _repository.UpdateAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteAsync_RemovesUser()
    {
        // Arrange
        var user = new User("test@example.com", "hash", "Test User", UserRole.Member);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var deletedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task GetByTeamIdAsync_ReturnsUsersInTeam()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var user1 = new User("user1@example.com", "hash", "User 1", UserRole.Member, team.Id);
        var user2 = new User("user2@example.com", "hash", "User 2", UserRole.Member, team.Id);
        var user3 = new User("user3@example.com", "hash", "User 3", UserRole.Member); // No team
        _context.Users.AddRange(user1, user2, user3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTeamIdAsync(team.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Email == "user1@example.com");
        result.Should().Contain(u => u.Email == "user2@example.com");
        result.Should().NotContain(u => u.Email == "user3@example.com");
    }

    [Fact]
    public async Task GetByTeamIdAsync_WithNoUsersInTeam_ReturnsEmptyCollection()
    {
        // Act
        var result = await _repository.GetByTeamIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}