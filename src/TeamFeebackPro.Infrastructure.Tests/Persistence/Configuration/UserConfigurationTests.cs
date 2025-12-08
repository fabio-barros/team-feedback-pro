using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;
using TeamFeedBackPro.Infrastructure.Persistence;

namespace TeamFeedbackPro.Infrastructure.Tests.Persistence.Configuration;

public class UserConfigurationTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public UserConfigurationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public void UserConfiguration_ShouldMapToUsersTable()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(User));

        // Assert
        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("users");
    }

    [Fact]
    public void UserConfiguration_IdProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(User));
        var idProperty = entityType!.FindProperty(nameof(User.Id));

        // Assert
        idProperty.Should().NotBeNull();
        idProperty!.GetColumnName().Should().Be("id");
        idProperty.ValueGenerated.Should().Be(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never);
        idProperty.IsKey().Should().BeTrue();
    }

    [Fact]
    public void UserConfiguration_EmailProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(User));
        var emailProperty = entityType!.FindProperty(nameof(User.Email));

        // Assert
        emailProperty.Should().NotBeNull();
        emailProperty!.GetColumnName().Should().Be("email");
        emailProperty.IsNullable.Should().BeFalse();
        emailProperty.GetMaxLength().Should().Be(255);
    }

    [Fact]
    public void UserConfiguration_EmailProperty_ShouldHaveUniqueIndex()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(User));
        var emailIndex = entityType!.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(User.Email)));

        // Assert
        emailIndex.Should().NotBeNull();
        emailIndex!.IsUnique.Should().BeTrue();
        emailIndex.GetDatabaseName().Should().Be("ix_users_email");
    }

    [Fact]
    public void UserConfiguration_PasswordHashProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(User));
        var passwordProperty = entityType!.FindProperty(nameof(User.PasswordHash));

        // Assert
        passwordProperty.Should().NotBeNull();
        passwordProperty!.GetColumnName().Should().Be("password_hash");
        passwordProperty.IsNullable.Should().BeFalse();
        passwordProperty.GetMaxLength().Should().Be(500);
    }

    [Fact]
    public void UserConfiguration_NameProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(User));
        var nameProperty = entityType!.FindProperty(nameof(User.Name));

        // Assert
        nameProperty.Should().NotBeNull();
        nameProperty!.GetColumnName().Should().Be("name");
        nameProperty.IsNullable.Should().BeFalse();
        nameProperty.GetMaxLength().Should().Be(200);
    }

    [Fact]
    public void UserConfiguration_RoleProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(User));
        var roleProperty = entityType!.FindProperty(nameof(User.Role));

        // Assert
        roleProperty.Should().NotBeNull();
        roleProperty!.GetColumnName().Should().Be("role");
        roleProperty.IsNullable.Should().BeFalse();
        roleProperty.GetMaxLength().Should().Be(50);
    }

    [Fact]
    public void UserConfiguration_RoleProperty_ShouldConvertEnumToString()
    {
        // Arrange
        var user = new User("test@example.com", "hash", "Test User", UserRole.Manager);
        _context.Users.Add(user);
        _context.SaveChanges();

        // Act
        var savedUser = _context.Users.First(u => u.Id == user.Id);

        // Assert
        savedUser.Role.Should().Be(UserRole.Manager);
    }

    [Fact]
    public void UserConfiguration_TeamIdProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(User));
        var teamIdProperty = entityType!.FindProperty(nameof(User.TeamId));

        // Assert
        teamIdProperty.Should().NotBeNull();
        teamIdProperty!.GetColumnName().Should().Be("team_id");
        teamIdProperty.IsNullable.Should().BeTrue();
    }

    [Fact]
    public void UserConfiguration_CreatedAtProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(User));
        var createdAtProperty = entityType!.FindProperty(nameof(User.CreatedAt));

        // Assert
        createdAtProperty.Should().NotBeNull();
        createdAtProperty!.GetColumnName().Should().Be("created_at");
        createdAtProperty.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void UserConfiguration_UpdatedAtProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(User));
        var updatedAtProperty = entityType!.FindProperty(nameof(User.UpdatedAt));

        // Assert
        updatedAtProperty.Should().NotBeNull();
        updatedAtProperty!.GetColumnName().Should().Be("updated_at");
        updatedAtProperty.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void UserConfiguration_TeamRelationship_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(User));
        var teamNavigation = entityType!.FindNavigation(nameof(User.Team));

        // Assert
        teamNavigation.Should().NotBeNull();
        teamNavigation!.ForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.SetNull);
    }

    [Fact]
    public async Task UserConfiguration_ShouldPersistAndRetrieveUser()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var user = new User("test@example.com", "hash123", "Test User", UserRole.Member, team.Id);

        // Act
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var retrievedUser = await _context.Users
            .Include(u => u.Team)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        // Assert
        retrievedUser.Should().NotBeNull();
        retrievedUser!.Email.Should().Be("test@example.com");
        retrievedUser.Name.Should().Be("Test User");
        retrievedUser.Role.Should().Be(UserRole.Member);
        retrievedUser.Team.Should().NotBeNull();
        retrievedUser.Team!.Name.Should().Be("Engineering");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}