using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Feedbacks.Queries.GetSentFeedbacks;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Feedbacks.Queries.GetSentFeedbacks;

public class GetSentFeedbacksQueryHandlerTests
{
    private readonly Mock<IFeedbackRepository> _feedbackRepositoryMock;
    private readonly Mock<ILogger<GetSentFeedbacksQueryHandler>> _loggerMock;
    private readonly GetSentFeedbacksQueryHandler _handler;

    public GetSentFeedbacksQueryHandlerTests()
    {
        _feedbackRepositoryMock = new Mock<IFeedbackRepository>();
        _loggerMock = new Mock<ILogger<GetSentFeedbacksQueryHandler>>();
        _handler = new GetSentFeedbacksQueryHandler(_feedbackRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithFeedbacks_ReturnsPaginatedResult()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);
        var feeling = new Feeling("Happy");

        var feedbacks = new List<Feedback>
        {
            new(authorId, Guid.NewGuid(), FeedbackType.Positive, FeedbackCategory.CodeQuality,
                "Great code quality demonstrated in recent work.", false, teamId, sprint.Id, feeling.Id),
            new(authorId, Guid.NewGuid(), FeedbackType.Constructive, FeedbackCategory.Communication,
                "Consider improving communication in stand-ups.", false, teamId, sprint.Id, null)
        };

        var query = new GetSentFeedbacksQuery(authorId, null, 1, 10);

        _feedbackRepositoryMock
            .Setup(x => x.GetSentByAuthorAsync(authorId, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((feedbacks, 2));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithNoFeedbacks_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var query = new GetSentFeedbacksQuery(authorId, null, 1, 10);

        _feedbackRepositoryMock
            .Setup(x => x.GetSentByAuthorAsync(authorId, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Feedback>(), 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithStatusFilter_FiltersCorrectly()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);

        var feedbacks = new List<Feedback>
        {
            new(authorId, Guid.NewGuid(), FeedbackType.Positive, FeedbackCategory.CodeQuality,
                "Approved feedback with sufficient content.", false, teamId, sprint.Id, null)
        };

        var query = new GetSentFeedbacksQuery(authorId, FeedbackStatus.Approved, 1, 10);

        _feedbackRepositoryMock
            .Setup(x => x.GetSentByAuthorAsync(authorId, FeedbackStatus.Approved, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((feedbacks, 1));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
        _feedbackRepositoryMock.Verify(
            x => x.GetSentByAuthorAsync(authorId, FeedbackStatus.Approved, 1, 10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithPagination_CalculatesTotalPagesCorrectly()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var query = new GetSentFeedbacksQuery(authorId, null, 2, 5);

        _feedbackRepositoryMock
            .Setup(x => x.GetSentByAuthorAsync(authorId, null, 2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Feedback>(), 12)); // 12 total items, 5 per page = 3 pages

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value!.TotalPages.Should().Be(3);
        result.Value.Page.Should().Be(2);
        result.Value.PageSize.Should().Be(5);
    }

    [Fact]
    public async Task Handle_MapsFeedbackToResult()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);
        var feeling = new Feeling("Happy");

        var feedback = new Feedback(
            authorId,
            recipientId,
            FeedbackType.Positive,
            FeedbackCategory.Leadership,
            "Demonstrated excellent leadership.",
            false,
            teamId,
            sprint.Id,
            feeling.Id
        );

        var query = new GetSentFeedbacksQuery(authorId, null, 1, 10);

        _feedbackRepositoryMock
            .Setup(x => x.GetSentByAuthorAsync(authorId, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Feedback> { feedback }, 1));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var feedbackResult = result.Value!.Items.First();
        feedbackResult.AuthorId.Should().Be(authorId);
        feedbackResult.RecipientId.Should().Be(recipientId);
        feedbackResult.Content.Should().Contain("leadership");
        feedbackResult.IsAnonymous.Should().BeFalse();
    }
}