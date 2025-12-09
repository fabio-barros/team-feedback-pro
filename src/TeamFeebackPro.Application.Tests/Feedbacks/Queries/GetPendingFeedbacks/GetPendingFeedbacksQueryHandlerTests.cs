using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Feedbacks.Queries.GetPendingFeedbacks;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;
using System.Reflection;

namespace TeamFeedbackPro.Application.Tests.Feedbacks.Queries.GetPendingFeedbacks;

public class GetPendingFeedbacksQueryHandlerTests
{
    private readonly Mock<IFeedbackRepository> _feedbackRepositoryMock;
    private readonly Mock<ILogger<GetPendingFeedbacksQueryHandler>> _loggerMock;
    private readonly GetPendingFeedbacksQueryHandler _handler;

    public GetPendingFeedbacksQueryHandlerTests()
    {
        _feedbackRepositoryMock = new Mock<IFeedbackRepository>();
        _loggerMock = new Mock<ILogger<GetPendingFeedbacksQueryHandler>>();
        _handler = new GetPendingFeedbacksQueryHandler(_feedbackRepositoryMock.Object, _loggerMock.Object);
    }

    private static void SetNavigationProperty<T>(T entity, string propertyName, object value)
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property != null && property.CanWrite)
        {
            property.SetValue(entity, value);
        }
        else
        {
            // Try to find backing field
            var field = typeof(T).GetField($"<{propertyName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(entity, value);
        }
    }

    [Fact]
    public async Task Handle_WithPendingFeedbacks_ReturnsPaginatedResult()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);

        var author = new User("author@test.com", "hash", "John Doe", UserRole.Member, teamId);
        var recipient = new User("recipient@test.com", "hash", "Jane Smith", UserRole.Member, teamId);

        var feedback = new Feedback(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great code quality demonstrated.",
            false,
            teamId,
            sprint.Id,
            null
        );

        // Set navigation properties using reflection
        SetNavigationProperty(feedback, "Author", author);
        SetNavigationProperty(feedback, "Recipient", recipient);
        SetNavigationProperty(feedback, "Sprint", sprint);

        var feedbacks = new List<Feedback> { feedback };
        var query = new GetPendingFeedbacksQuery(managerId, teamId, 1, 10);

        _feedbackRepositoryMock
            .Setup(x => x.GetPendingByManagerAsync(teamId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((feedbacks, 1));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_WithNoPendingFeedbacks_ReturnsEmptyResult()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var query = new GetPendingFeedbacksQuery(managerId, teamId, 1, 10);

        _feedbackRepositoryMock
            .Setup(x => x.GetPendingByManagerAsync(teamId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Feedback>(), 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_IncludesAuthorAndRecipientNames()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);

        var author = new User("author@test.com", "hash", "John Doe", UserRole.Member, teamId);
        var recipient = new User("recipient@test.com", "hash", "Jane Smith", UserRole.Member, teamId);

        var feedback = new Feedback(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.Leadership,
            "Demonstrated strong leadership.",
            false,
            teamId,
            sprint.Id,
            null
        );

        // Set navigation properties
        SetNavigationProperty(feedback, "Author", author);
        SetNavigationProperty(feedback, "Recipient", recipient);
        SetNavigationProperty(feedback, "Sprint", sprint);

        var query = new GetPendingFeedbacksQuery(managerId, teamId, 1, 10);

        _feedbackRepositoryMock
            .Setup(x => x.GetPendingByManagerAsync(teamId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Feedback> { feedback }, 1));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var feedbackResult = result.Value!.Items.First();
        feedbackResult.AuthorName.Should().Be("John Doe");
        feedbackResult.RecipientName.Should().Be("Jane Smith");
    }

    [Fact]
    public async Task Handle_CalculatesTotalPagesCorrectly()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var query = new GetPendingFeedbacksQuery(managerId, teamId, 1, 7);

        _feedbackRepositoryMock
            .Setup(x => x.GetPendingByManagerAsync(teamId, 1, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Feedback>(), 15)); // 15 items, 7 per page = 3 pages

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value!.TotalPages.Should().Be(3);
    }
}