using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;
using TeamFeedBackPro.Infrastructure.Persistence;

namespace TeamFeedbackPro.Infrastructure.Tests.Persistence.Configuration;

public class TeamConfigurationTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public TeamConfigurationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public void TeamConfiguration_ShouldMapToTeamsTable()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Team));

        // Assert
        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("teams");
    }

    [Fact]
    public void TeamConfiguration_IdProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Team));
        var idProperty = entityType!.FindProperty(nameof(Team.Id));

        // Assert
        idProperty.Should().NotBeNull();
        idProperty!.GetColumnName().Should().Be("id");
        idProperty.ValueGenerated.Should().Be(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never);
        idProperty.IsKey().Should().BeTrue();
    }

    [Fact]
    public void TeamConfiguration_NameProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Team));
        var nameProperty = entityType!.FindProperty(nameof(Team.Name));

        // Assert
        nameProperty.Should().NotBeNull();
        nameProperty!.GetColumnName().Should().Be("name");
        nameProperty.IsNullable.Should().BeFalse();
        nameProperty.GetMaxLength().Should().Be(200);
    }

    [Fact]
    public void TeamConfiguration_ManagerIdProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Team));
        var managerIdProperty = entityType!.FindProperty(nameof(Team.ManagerId));

        // Assert
        managerIdProperty.Should().NotBeNull();
        managerIdProperty!.GetColumnName().Should().Be("manager_id");
        managerIdProperty.IsNullable.Should().BeTrue();
    }

    [Fact]
    public void TeamConfiguration_CreatedAtProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Team));
        var createdAtProperty = entityType!.FindProperty(nameof(Team.CreatedAt));

        // Assert
        createdAtProperty.Should().NotBeNull();
        createdAtProperty!.GetColumnName().Should().Be("created_at");
        createdAtProperty.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void TeamConfiguration_UpdatedAtProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Team));
        var updatedAtProperty = entityType!.FindProperty(nameof(Team.UpdatedAt));

        // Assert
        updatedAtProperty.Should().NotBeNull();
        updatedAtProperty!.GetColumnName().Should().Be("updated_at");
        updatedAtProperty.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void TeamConfiguration_MembersRelationship_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Team));
        var membersNavigation = entityType!.FindNavigation(nameof(Team.Members));

        // Assert
        membersNavigation.Should().NotBeNull();
        membersNavigation!.IsCollection.Should().BeTrue();
        membersNavigation.ForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.SetNull);
    }

    [Fact]
    public async Task TeamConfiguration_ShouldPersistAndRetrieveTeam()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var team = new Team("Engineering Team", managerId);

        // Act
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var retrievedTeam = await _context.Teams.FirstOrDefaultAsync(t => t.Id == team.Id);

        // Assert
        retrievedTeam.Should().NotBeNull();
        retrievedTeam!.Name.Should().Be("Engineering Team");
        retrievedTeam.ManagerId.Should().Be(managerId);
    }

    [Fact]
    public async Task TeamConfiguration_ShouldLoadMembersCollection()
    {
        // Arrange
        var team = new Team("Engineering Team");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var user1 = new User("user1@example.com", "hash", "User 1", UserRole.Member, team.Id);
        var user2 = new User("user2@example.com", "hash", "User 2", UserRole.Member, team.Id);
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        // Act
        var retrievedTeam = await _context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == team.Id);

        // Assert
        retrievedTeam.Should().NotBeNull();
        retrievedTeam!.Members.Should().HaveCount(2);
        retrievedTeam.Members.Should().Contain(u => u.Email == "user1@example.com");
        retrievedTeam.Members.Should().Contain(u => u.Email == "user2@example.com");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}