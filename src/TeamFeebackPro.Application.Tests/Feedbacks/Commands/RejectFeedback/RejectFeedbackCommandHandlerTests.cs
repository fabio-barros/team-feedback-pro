using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Feedbacks.Commands.RejectFeedback;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Feedbacks.Commands.RejectFeedback;

public class RejectFeedbackCommandHandlerTests
{
    private readonly Mock<IFeedbackRepository> _feedbackRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RejectFeedbackCommandHandler>> _loggerMock;
    private readonly RejectFeedbackCommandHandler _handler;

    public RejectFeedbackCommandHandlerTests()
    {
        _feedbackRepositoryMock = new Mock<IFeedbackRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RejectFeedbackCommandHandler>>();

        _handler = new RejectFeedbackCommandHandler(
            _feedbackRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccess()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var author = new User("author@test.com", "hash", "Author", UserRole.Member, teamId);
        var recipient = new User("recipient@test.com", "hash", "Recipient", UserRole.Member, teamId);
        var manager = new User("manager@test.com", "hash", "Manager", UserRole.Manager, teamId);
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);

        var feedback = new Feedback(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great code quality demonstrated in recent work.",
            false,
            teamId,
            sprint.Id
        );

        var command = new RejectFeedbackCommand(feedback.Id, manager.Id, "Content needs improvement for better clarity.");

        _feedbackRepositoryMock.Setup(x => x.GetByIdAsync(feedback.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(manager.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(recipient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipient);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistingFeedback_ReturnsFailure()
    {
        // Arrange
        var command = new RejectFeedbackCommand(Guid.NewGuid(), Guid.NewGuid(), "Rejection notes here with sufficient length.");

        _feedbackRepositoryMock.Setup(x => x.GetByIdAsync(command.FeedbackId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Feedback?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Feedback not found");
    }

    [Fact]
    public async Task Handle_WithNonPendingFeedback_ReturnsFailure()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var author = new User("author@test.com", "hash", "Author", UserRole.Member, teamId);
        var recipient = new User("recipient@test.com", "hash", "Recipient", UserRole.Member, teamId);
        var manager = new User("manager@test.com", "hash", "Manager", UserRole.Manager, teamId);
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);

        var feedback = new Feedback(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great code quality demonstrated.",
            false,
            teamId,
            sprint.Id
        );

        // Reject it first
        feedback.Reject(manager.Id, "Already rejected with sufficient text.");

        var command = new RejectFeedbackCommand(feedback.Id, manager.Id, "Trying to reject again with more text.");

        _feedbackRepositoryMock.Setup(x => x.GetByIdAsync(feedback.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Feedback is not pending");
    }

    [Fact]
    public async Task Handle_CallsSaveChanges()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var author = new User("author@test.com", "hash", "Author", UserRole.Member, teamId);
        var recipient = new User("recipient@test.com", "hash", "Recipient", UserRole.Member, teamId);
        var manager = new User("manager@test.com", "hash", "Manager", UserRole.Manager, teamId);
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);

        var feedback = new Feedback(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great code quality demonstrated.",
            false,
            teamId,
            sprint.Id
        );

        var command = new RejectFeedbackCommand(feedback.Id, manager.Id, "Rejected due to insufficient detail.");

        _feedbackRepositoryMock.Setup(x => x.GetByIdAsync(feedback.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(manager.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(recipient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipient);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}