using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;
using TeamFeedBackPro.Infrastructure.Persistence;
using TeamFeedBackPro.Infrastructure.Persistence.Repositories;

namespace TeamFeedbackPro.Infrastructure.Tests.Persistence.Repositories;

public class FeedbackRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly FeedbackRepository _repository;

    public FeedbackRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new FeedbackRepository(_context);
    }

    private async Task<(Team team, User author, User recipient, Sprint sprint)> SeedTestDataAsync()
    {
        var team = new Team("Engineering");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var sprint = new Sprint("Sprint 1", "Current sprint", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), team.Id);
        _context.Sprints.Add(sprint);
        await _context.SaveChangesAsync();

        var author = new User("author@example.com", "hash", "Author", UserRole.Member, team.Id);
        var recipient = new User("recipient@example.com", "hash", "Recipient", UserRole.Member, team.Id);
        _context.Users.AddRange(author, recipient);
        await _context.SaveChangesAsync();

        return (team, author, recipient, sprint);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingFeedback_ReturnsFeedback()
    {
        // Arrange
        var (team, author, recipient, sprint) = await SeedTestDataAsync();
        var feedback = new Feedback(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great work on the code review!",
            false,
            team.Id,
            sprint.Id
        );
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(feedback.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(feedback.Id);
        result.Content.Should().Contain("Great work");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingFeedback_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_IncludesNavigationProperties()
    {
        // Arrange
        var (team, author, recipient, sprint) = await SeedTestDataAsync();
        var feedback = new Feedback(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great work on the code review!",
            false,
            team.Id,
            sprint.Id
        );
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(feedback.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Author.Should().NotBeNull();
        result.Author!.Email.Should().Be("author@example.com");
        result.Recipient.Should().NotBeNull();
        result.Recipient!.Email.Should().Be("recipient@example.com");
        result.Reviewer.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_AddsFeedbackToContext()
    {
        // Arrange
        var (team, author, recipient, sprint) = await SeedTestDataAsync();
        var feedback = new Feedback(
            author.Id,
            recipient.Id,
            FeedbackType.Constructive,
            FeedbackCategory.Communication,
            "Consider being more clear in your communication during meetings.",
            false,
            team.Id,
            sprint.Id
        );

        // Act
        var result = await _repository.AddAsync(feedback);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().Be(feedback);
        var savedFeedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == feedback.Id);
        savedFeedback.Should().NotBeNull();
        savedFeedback!.Content.Should().Contain("Consider being more clear");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFeedback()
    {
        // Arrange
        var (team, author, recipient, sprint) = await SeedTestDataAsync();
        var reviewer = new User("reviewer@example.com", "hash", "Reviewer", UserRole.Manager, team.Id);
        _context.Users.Add(reviewer);
        await _context.SaveChangesAsync();

        var feedback = new Feedback(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.Leadership,
            "Excellent leadership during the project sprint.",
            false,
            team.Id,
            sprint.Id
        );
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        // Act
        feedback.Approve(reviewer.Id, "Approved feedback");
        await _repository.UpdateAsync(feedback);
        await _context.SaveChangesAsync();

        // Assert
        var updatedFeedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == feedback.Id);
        updatedFeedback.Should().NotBeNull();
        updatedFeedback!.Status.Should().Be(FeedbackStatus.Approved);
        updatedFeedback.ReviewedBy.Should().Be(reviewer.Id);
        updatedFeedback.ReviewNotes.Should().Be("Approved feedback");
    }

    [Fact]
    public async Task GetSentByAuthorAsync_ReturnsAuthorFeedbacks()
    {
        // Arrange
        var (team, author, recipient, sprint) = await SeedTestDataAsync();
        var feedback1 = new Feedback(author.Id, recipient.Id, FeedbackType.Positive, FeedbackCategory.CodeQuality, "Great code quality demonstrated in recent PRs.", false, team.Id, sprint.Id);
        var feedback2 = new Feedback(author.Id, recipient.Id, FeedbackType.Constructive, FeedbackCategory.Communication, "Consider improving meeting attendance.", false, team.Id, sprint.Id);
        _context.Feedbacks.AddRange(feedback1, feedback2);
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetSentByAuthorAsync(author.Id, null, 1, 10);

        // Assert
        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
        items.Should().Contain(f => f.Content.Contains("Great code"));
        items.Should().Contain(f => f.Content.Contains("improving meeting"));
    }

    [Fact]
    public async Task GetSentByAuthorAsync_WithStatusFilter_ReturnsFilteredFeedbacks()
    {
        // Arrange
        var (team, author, recipient, sprint) = await SeedTestDataAsync();
        var reviewer = new User("reviewer@example.com", "hash", "Reviewer", UserRole.Manager, team.Id);
        _context.Users.Add(reviewer);
        await _context.SaveChangesAsync();

        var feedback1 = new Feedback(author.Id, recipient.Id, FeedbackType.Positive, FeedbackCategory.CodeQuality, "Great code quality demonstrated.", false, team.Id, sprint.Id);
        var feedback2 = new Feedback(author.Id, recipient.Id, FeedbackType.Constructive, FeedbackCategory.Communication, "Consider improving communication.", false, team.Id, sprint.Id);
        _context.Feedbacks.AddRange(feedback1, feedback2);
        await _context.SaveChangesAsync();

        feedback1.Approve(reviewer.Id);
        _context.Feedbacks.Update(feedback1);
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetSentByAuthorAsync(author.Id, FeedbackStatus.Approved, 1, 10);

        // Assert
        totalCount.Should().Be(1);
        items.Should().HaveCount(1);
        items.First().Status.Should().Be(FeedbackStatus.Approved);
    }

    [Fact]
    public async Task GetSentByAuthorAsync_WithPagination_ReturnsPaginatedResults()
    {
        // Arrange
        var (team, author, recipient, sprint) = await SeedTestDataAsync();
        for (int i = 0; i < 5; i++)
        {
            var feedback = new Feedback(author.Id, recipient.Id, FeedbackType.Positive, FeedbackCategory.CodeQuality, $"Feedback {i} with sufficient content length.", false, team.Id, sprint.Id);
            _context.Feedbacks.Add(feedback);
        }
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetSentByAuthorAsync(author.Id, null, 2, 2);

        // Assert
        totalCount.Should().Be(5);
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetReceviedByUserAsync_ReturnsApprovedFeedbacks()
    {
        // Arrange
        var (team, author, recipient, sprint) = await SeedTestDataAsync();
        var reviewer = new User("reviewer@example.com", "hash", "Reviewer", UserRole.Manager, team.Id);
        _context.Users.Add(reviewer);
        await _context.SaveChangesAsync();

        var feedback1 = new Feedback(author.Id, recipient.Id, FeedbackType.Positive, FeedbackCategory.Teamwork, "Excellent teamwork on the recent project.", false, team.Id, sprint.Id);
        var feedback2 = new Feedback(author.Id, recipient.Id, FeedbackType.Constructive, FeedbackCategory.ProblemSolving, "Great problem solving skills shown.", false, team.Id, sprint.Id);
        _context.Feedbacks.AddRange(feedback1, feedback2);
        await _context.SaveChangesAsync();

        feedback1.Approve(reviewer.Id);
        _context.Feedbacks.Update(feedback1);
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetReceviedByUserAsync(recipient.Id, null, 1, 10);

        // Assert
        totalCount.Should().Be(1);
        items.Should().HaveCount(1);
        items.First().Status.Should().Be(FeedbackStatus.Approved);
    }

    [Fact]
    public async Task GetPendingByManagerAsync_ReturnsPendingFeedbacksForTeam()
    {
        // Arrange
        var (team, author, recipient, sprint) = await SeedTestDataAsync();
        var feedback1 = new Feedback(author.Id, recipient.Id, FeedbackType.Positive, FeedbackCategory.Leadership, "Demonstrated strong leadership qualities.", false, team.Id, sprint.Id);
        var feedback2 = new Feedback(author.Id, recipient.Id, FeedbackType.Constructive, FeedbackCategory.Other, "Other feedback with sufficient content.", false, team.Id, sprint.Id);
        _context.Feedbacks.AddRange(feedback1, feedback2);
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetPendingByManagerAsync(team.Id, 1, 10);

        // Assert
        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
        items.Should().OnlyContain(f => f.Status == FeedbackStatus.Pending);
    }

    [Fact]
    public async Task GetPendingByRecipientAsync_ReturnsPendingFeedbacksForRecipient()
    {
        // Arrange
        var (team, author, recipient, sprint) = await SeedTestDataAsync();
        var feedback1 = new Feedback(author.Id, recipient.Id, FeedbackType.Positive, FeedbackCategory.CodeQuality, "Excellent code quality in recent work.", false, team.Id, sprint.Id);
        var feedback2 = new Feedback(author.Id, recipient.Id, FeedbackType.Constructive, FeedbackCategory.Communication, "Communication could be more proactive.", false, team.Id, sprint.Id);
        _context.Feedbacks.AddRange(feedback1, feedback2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPendingByRecipientAsync(recipient.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(f => f.Status == FeedbackStatus.Pending);
        result.Should().OnlyContain(f => f.RecipientId == recipient.Id);
    }

    [Fact]
    public async Task GetSentByAuthorAsync_OrdersByCreatedAtDescending()
    {
        // Arrange
        var (team, author, recipient, sprint) = await SeedTestDataAsync();
        var feedback1 = new Feedback(author.Id, recipient.Id, FeedbackType.Positive, FeedbackCategory.CodeQuality, "First feedback with sufficient content length here.", false, team.Id, sprint.Id);
        _context.Feedbacks.Add(feedback1);
        await _context.SaveChangesAsync();

        await Task.Delay(100); // Ensure different timestamps

        var feedback2 = new Feedback(author.Id, recipient.Id, FeedbackType.Constructive, FeedbackCategory.Communication, "Second feedback with sufficient content length.", false, team.Id, sprint.Id);
        _context.Feedbacks.Add(feedback2);
        await _context.SaveChangesAsync();

        // Act
        var (items, _) = await _repository.GetSentByAuthorAsync(author.Id, null, 1, 10);

        // Assert
        var itemsList = items.ToList();
        itemsList[0].CreatedAt.Should().BeAfter(itemsList[1].CreatedAt);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}