using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Feedbacks.Queries.GetReceivedFeedbacks;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Feedbacks.Queries.GetReceivedFeedbacks;

public class GetReceivedFeedbacksQueryHandlerTests
{
    private readonly Mock<IFeedbackRepository> _feedbackRepositoryMock;
    private readonly Mock<ILogger<GetReceivedFeedbacksQueryHandler>> _loggerMock;
    private readonly GetReceivedFeedbacksQueryHandler _handler;

    public GetReceivedFeedbacksQueryHandlerTests()
    {
        _feedbackRepositoryMock = new Mock<IFeedbackRepository>();
        _loggerMock = new Mock<ILogger<GetReceivedFeedbacksQueryHandler>>();
        _handler = new GetReceivedFeedbacksQueryHandler(_feedbackRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithFeedbacks_ReturnsPaginatedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);

        var feedbacks = new List<Feedback>
        {
            new(Guid.NewGuid(), userId, FeedbackType.Positive, FeedbackCategory.Teamwork,
                "Excellent teamwork on the recent project.", false, teamId, sprint.Id, null),
            new(Guid.NewGuid(), userId, FeedbackType.Constructive, FeedbackCategory.Communication,
                "Consider being more proactive in communications.", false, teamId, sprint.Id, null)
        };

        var query = new GetReceivedFeedbacksQuery(userId, null, 1, 10);

        _feedbackRepositoryMock
            .Setup(x => x.GetReceviedByUserAsync(userId, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((feedbacks, 2));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoFeedbacks_ReturnsEmptyResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetReceivedFeedbacksQuery(userId, null, 1, 10);

        _feedbackRepositoryMock
            .Setup(x => x.GetReceviedByUserAsync(userId, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Feedback>(), 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithStatusFilter_FiltersCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetReceivedFeedbacksQuery(userId, FeedbackStatus.Approved, 1, 10);

        _feedbackRepositoryMock
            .Setup(x => x.GetReceviedByUserAsync(userId, FeedbackStatus.Approved, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Feedback>(), 0));

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _feedbackRepositoryMock.Verify(
            x => x.GetReceviedByUserAsync(userId, FeedbackStatus.Approved, 1, 10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_CalculatesTotalPagesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetReceivedFeedbacksQuery(userId, null, 1, 3);

        _feedbackRepositoryMock
            .Setup(x => x.GetReceviedByUserAsync(userId, null, 1, 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Feedback>(), 10)); // 10 items, 3 per page = 4 pages

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value!.TotalPages.Should().Be(4);
    }
}