using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Feedbacks.Commands.ApproveFeedback;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Feedbacks.Commands.ApproveFeedback;

public class ApproveFeedbackCommandHandlerTests
{
    private readonly Mock<IFeedbackRepository> _feedbackRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<ApproveFeedbackCommandHandler>> _loggerMock;
    private readonly ApproveFeedbackCommandHandler _handler;

    public ApproveFeedbackCommandHandlerTests()
    {
        _feedbackRepositoryMock = new Mock<IFeedbackRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<ApproveFeedbackCommandHandler>>();

        _handler = new ApproveFeedbackCommandHandler(
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

        var command = new ApproveFeedbackCommand(feedback.Id, manager.Id, "Looks good!");

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
        var command = new ApproveFeedbackCommand(Guid.NewGuid(), Guid.NewGuid(), null);

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

        // Approve it first
        feedback.Approve(manager.Id);

        var command = new ApproveFeedbackCommand(feedback.Id, manager.Id, null);

        _feedbackRepositoryMock.Setup(x => x.GetByIdAsync(feedback.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Feedback is not pending");
    }

    [Fact]
    public async Task Handle_WithNonExistingManager_ReturnsFailure()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var author = new User("author@test.com", "hash", "Author", UserRole.Member, teamId);
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);

        var feedback = new Feedback(
            author.Id,
            Guid.NewGuid(),
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great code quality demonstrated.",
            false,
            teamId,
            sprint.Id
        );

        var command = new ApproveFeedbackCommand(feedback.Id, Guid.NewGuid(), null);

        _feedbackRepositoryMock.Setup(x => x.GetByIdAsync(feedback.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(command.ManagerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_WithNonExistingAuthor_ReturnsFailure()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var manager = new User("manager@test.com", "hash", "Manager", UserRole.Manager, teamId);
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);

        var feedback = new Feedback(
            Guid.NewGuid(),
            Guid.NewGuid(),
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great code quality demonstrated.",
            false,
            teamId,
            sprint.Id
        );

        var command = new ApproveFeedbackCommand(feedback.Id, manager.Id, null);

        _feedbackRepositoryMock.Setup(x => x.GetByIdAsync(feedback.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(manager.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(feedback.AuthorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_WithNonExistingRecipient_ReturnsFailure()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var author = new User("author@test.com", "hash", "Author", UserRole.Member, teamId);
        var manager = new User("manager@test.com", "hash", "Manager", UserRole.Manager, teamId);
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);

        var feedback = new Feedback(
            author.Id,
            Guid.NewGuid(),
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great code quality demonstrated.",
            false,
            teamId,
            sprint.Id
        );

        var command = new ApproveFeedbackCommand(feedback.Id, manager.Id, null);

        _feedbackRepositoryMock.Setup(x => x.GetByIdAsync(feedback.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(manager.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(feedback.RecipientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_WithDifferentTeams_ReturnsFailure()
    {
        // Arrange
        var teamId1 = Guid.NewGuid();
        var teamId2 = Guid.NewGuid();
        var author = new User("author@test.com", "hash", "Author", UserRole.Member, teamId1);
        var recipient = new User("recipient@test.com", "hash", "Recipient", UserRole.Member, teamId2);
        var manager = new User("manager@test.com", "hash", "Manager", UserRole.Manager, teamId1);
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId1);

        var feedback = new Feedback(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great code quality demonstrated.",
            false,
            teamId1,
            sprint.Id
        );

        var command = new ApproveFeedbackCommand(feedback.Id, manager.Id, null);

        _feedbackRepositoryMock.Setup(x => x.GetByIdAsync(feedback.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(manager.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(recipient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipient);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Author and recipient must be in the same team");
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

        var command = new ApproveFeedbackCommand(feedback.Id, manager.Id, "Approved");

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