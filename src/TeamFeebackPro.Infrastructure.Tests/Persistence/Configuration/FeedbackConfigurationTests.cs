using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;
using TeamFeedBackPro.Infrastructure.Persistence;

namespace TeamFeedbackPro.Infrastructure.Tests.Persistence.Configuration;

public class FeedbackConfigurationTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public FeedbackConfigurationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public void FeedbackConfiguration_ShouldMapToFeedbacksTable()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));

        // Assert
        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("feedbacks");
    }

    [Fact]
    public void FeedbackConfiguration_IdProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var idProperty = entityType!.FindProperty(nameof(Feedback.Id));

        // Assert
        idProperty.Should().NotBeNull();
        idProperty!.GetColumnName().Should().Be("id");
        idProperty.ValueGenerated.Should().Be(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never);
        idProperty.IsKey().Should().BeTrue();
    }

    [Fact]
    public void FeedbackConfiguration_AuthorIdProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var authorIdProperty = entityType!.FindProperty(nameof(Feedback.AuthorId));

        // Assert
        authorIdProperty.Should().NotBeNull();
        authorIdProperty!.GetColumnName().Should().Be("author_id");
        authorIdProperty.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void FeedbackConfiguration_RecipientIdProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var recipientIdProperty = entityType!.FindProperty(nameof(Feedback.RecipientId));

        // Assert
        recipientIdProperty.Should().NotBeNull();
        recipientIdProperty!.GetColumnName().Should().Be("recipient_id");
        recipientIdProperty.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void FeedbackConfiguration_TypeProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var typeProperty = entityType!.FindProperty(nameof(Feedback.Type));

        // Assert
        typeProperty.Should().NotBeNull();
        typeProperty!.GetColumnName().Should().Be("type");
        typeProperty.IsNullable.Should().BeFalse();
        typeProperty.GetMaxLength().Should().Be(50);
    }

    [Fact]
    public void FeedbackConfiguration_CategoryProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var categoryProperty = entityType!.FindProperty(nameof(Feedback.Category));

        // Assert
        categoryProperty.Should().NotBeNull();
        categoryProperty!.GetColumnName().Should().Be("category");
        categoryProperty.IsNullable.Should().BeFalse();
        categoryProperty.GetMaxLength().Should().Be(50);
    }

    [Fact]
    public void FeedbackConfiguration_ContentProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var contentProperty = entityType!.FindProperty(nameof(Feedback.Content));

        // Assert
        contentProperty.Should().NotBeNull();
        contentProperty!.GetColumnName().Should().Be("content");
        contentProperty.IsNullable.Should().BeFalse();
        contentProperty.GetMaxLength().Should().Be(2000);
    }

    [Fact]
    public void FeedbackConfiguration_IsAnonymousProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var isAnonymousProperty = entityType!.FindProperty(nameof(Feedback.IsAnonymous));

        // Assert
        isAnonymousProperty.Should().NotBeNull();
        isAnonymousProperty!.GetColumnName().Should().Be("is_anonymous");
        isAnonymousProperty.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void FeedbackConfiguration_StatusProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var statusProperty = entityType!.FindProperty(nameof(Feedback.Status));

        // Assert
        statusProperty.Should().NotBeNull();
        statusProperty!.GetColumnName().Should().Be("status");
        statusProperty.IsNullable.Should().BeFalse();
        statusProperty.GetMaxLength().Should().Be(50);
    }

    [Fact]
    public void FeedbackConfiguration_ReviewedByProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var reviewedByProperty = entityType!.FindProperty(nameof(Feedback.ReviewedBy));

        // Assert
        reviewedByProperty.Should().NotBeNull();
        reviewedByProperty!.GetColumnName().Should().Be("reviewed_by");
        reviewedByProperty.IsNullable.Should().BeTrue();
    }

    [Fact]
    public void FeedbackConfiguration_ReviewedAtProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var reviewedAtProperty = entityType!.FindProperty(nameof(Feedback.ReviewedAt));

        // Assert
        reviewedAtProperty.Should().NotBeNull();
        reviewedAtProperty!.GetColumnName().Should().Be("reviewed_at");
        reviewedAtProperty.IsNullable.Should().BeTrue();
    }

    [Fact]
    public void FeedbackConfiguration_ReviewNotesProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var reviewNotesProperty = entityType!.FindProperty(nameof(Feedback.ReviewNotes));

        // Assert
        reviewNotesProperty.Should().NotBeNull();
        reviewNotesProperty!.GetColumnName().Should().Be("review_notes");
        reviewNotesProperty.GetMaxLength().Should().Be(500);
        reviewNotesProperty.IsNullable.Should().BeTrue();
    }

    [Fact]
    public void FeedbackConfiguration_TeamIdProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var teamIdProperty = entityType!.FindProperty(nameof(Feedback.TeamId));

        // Assert
        teamIdProperty.Should().NotBeNull();
        teamIdProperty!.GetColumnName().Should().Be("team_id");
        teamIdProperty.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void FeedbackConfiguration_SprintIdProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var sprintIdProperty = entityType!.FindProperty(nameof(Feedback.SprintId));

        // Assert
        sprintIdProperty.Should().NotBeNull();
        sprintIdProperty!.GetColumnName().Should().Be("sprint_id");
        sprintIdProperty.IsNullable.Should().BeTrue();
    }

    [Fact]
    public void FeedbackConfiguration_FeelingIdProperty_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var feelingIdProperty = entityType!.FindProperty(nameof(Feedback.FeelingId));

        // Assert
        feelingIdProperty.Should().NotBeNull();
        feelingIdProperty!.GetColumnName().Should().Be("feeling_id");
        feelingIdProperty.IsNullable.Should().BeTrue();
    }

    [Fact]
    public void FeedbackConfiguration_AuthorRelationship_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var authorNavigation = entityType!.FindNavigation(nameof(Feedback.Author));

        // Assert
        authorNavigation.Should().NotBeNull();
        authorNavigation!.ForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);
    }

    [Fact]
    public void FeedbackConfiguration_RecipientRelationship_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var recipientNavigation = entityType!.FindNavigation(nameof(Feedback.Recipient));

        // Assert
        recipientNavigation.Should().NotBeNull();
        recipientNavigation!.ForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);
    }

    [Fact]
    public void FeedbackConfiguration_ReviewerRelationship_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var reviewerNavigation = entityType!.FindNavigation(nameof(Feedback.Reviewer));

        // Assert
        reviewerNavigation.Should().NotBeNull();
        reviewerNavigation!.ForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.SetNull);
    }

    [Fact]
    public void FeedbackConfiguration_TeamRelationship_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var teamNavigation = entityType!.FindNavigation(nameof(Feedback.Team));

        // Assert
        teamNavigation.Should().NotBeNull();
        teamNavigation!.ForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.SetNull);
    }

    [Fact]
    public void FeedbackConfiguration_SprintRelationship_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var sprintNavigation = entityType!.FindNavigation(nameof(Feedback.Sprint));

        // Assert
        sprintNavigation.Should().NotBeNull();
        sprintNavigation!.ForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);
    }

    [Fact]
    public void FeedbackConfiguration_FeelingRelationship_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var feelingNavigation = entityType!.FindNavigation(nameof(Feedback.Feeling));

        // Assert
        feelingNavigation.Should().NotBeNull();
        feelingNavigation!.ForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.SetNull);
    }

    [Theory]
    [InlineData("ix_feedbacks_author_id")]
    [InlineData("ix_feedbacks_recipient_id")]
    [InlineData("ix_feedbacks_status")]
    [InlineData("ix_feedbacks_created_at")]
    public void FeedbackConfiguration_ShouldHaveIndex(string indexName)
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(Feedback));
        var indexes = entityType!.GetIndexes();

        // Assert
        indexes.Should().Contain(i => i.GetDatabaseName() == indexName);
    }

    [Fact]
    public async Task FeedbackConfiguration_ShouldPersistAndRetrieveFeedback()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var sprint = new Sprint("Sprint 1", "Test sprint description", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), team.Id);
        _context.Sprints.Add(sprint);
        await _context.SaveChangesAsync();

        var author = new User("author@example.com", "hash", "Author", UserRole.Member, team.Id);
        var recipient = new User("recipient@example.com", "hash", "Recipient", UserRole.Member, team.Id);
        _context.Users.AddRange(author, recipient);
        await _context.SaveChangesAsync();

        var feedback = new Feedback(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great work on the code review! Very thorough and helpful.",
            false,
            team.Id,
            sprint.Id
        );

        // Act
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        var retrievedFeedback = await _context.Feedbacks
            .Include(f => f.Author)
            .Include(f => f.Recipient)
            .Include(f => f.Team)
            .Include(f => f.Sprint)
            .FirstOrDefaultAsync(f => f.Id == feedback.Id);

        // Assert
        retrievedFeedback.Should().NotBeNull();
        retrievedFeedback!.Content.Should().Contain("Great work");
        retrievedFeedback.Type.Should().Be(FeedbackType.Positive);
        retrievedFeedback.Category.Should().Be(FeedbackCategory.CodeQuality);
        retrievedFeedback.Status.Should().Be(FeedbackStatus.Pending);
        retrievedFeedback.Author.Should().NotBeNull();
        retrievedFeedback.Author!.Email.Should().Be("author@example.com");
        retrievedFeedback.Recipient.Should().NotBeNull();
        retrievedFeedback.Recipient!.Email.Should().Be("recipient@example.com");
        retrievedFeedback.Team.Should().NotBeNull();
        retrievedFeedback.Team!.Name.Should().Be("Engineering");
        retrievedFeedback.Sprint.Should().NotBeNull();
        retrievedFeedback.Sprint!.Name.Should().Be("Sprint 1");
    }

    [Fact]
    public async Task FeedbackConfiguration_EnumProperties_ShouldConvertToString()
    {
        // Arrange
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var sprint = new Sprint("Sprint 1", "Test sprint description", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), team.Id);
        _context.Sprints.Add(sprint);
        await _context.SaveChangesAsync();

        var author = new User("author@example.com", "hash", "Author", UserRole.Member, team.Id);
        var recipient = new User("recipient@example.com", "hash", "Recipient", UserRole.Member, team.Id);
        _context.Users.AddRange(author, recipient);
        await _context.SaveChangesAsync();

        var feedback = new Feedback(
            author.Id,
            recipient.Id,
            FeedbackType.Constructive,
            FeedbackCategory.Leadership,
            "Consider being more assertive in meetings to share your valuable insights.",
            false,
            team.Id,
            sprint.Id
        );

        // Act
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        var retrievedFeedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == feedback.Id);

        // Assert
        retrievedFeedback.Should().NotBeNull();
        retrievedFeedback!.Type.Should().Be(FeedbackType.Constructive);
        retrievedFeedback.Category.Should().Be(FeedbackCategory.Leadership);
        retrievedFeedback.Status.Should().Be(FeedbackStatus.Pending);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}