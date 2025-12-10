using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Feedbacks.Commands.CreateFeedback;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Feedbacks.Commands.CreateFeedback;

public class CreateFeedbackCommandHandlerTests
{
    private readonly Mock<IFeedbackRepository> _feedbackRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IFeelingRepository> _feelingRepositoryMock;
    private readonly Mock<ISprintRepository> _sprintRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateFeedbackCommandHandler>> _loggerMock;
    private readonly CreateFeedbackCommandHandler _handler;

    public CreateFeedbackCommandHandlerTests()
    {
        _feedbackRepositoryMock = new Mock<IFeedbackRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _feelingRepositoryMock = new Mock<IFeelingRepository>();
        _sprintRepositoryMock = new Mock<ISprintRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateFeedbackCommandHandler>>();

        _handler = new CreateFeedbackCommandHandler(
            _feedbackRepositoryMock.Object,
            _userRepositoryMock.Object,
            _feelingRepositoryMock.Object,
            _sprintRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var author = new User("author@test.com", "hash", "Author", UserRole.Member, teamId);
        var recipient = new User("recipient@test.com", "hash", "Recipient", UserRole.Member, teamId);
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);

        var command = new CreateFeedbackCommand(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great code quality demonstrated in recent work.",
            false,
            null
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(recipient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipient);
        _sprintRepositoryMock.Setup(x => x.GetActualSprintAsync(teamId))
            .ReturnsAsync(sprint);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Content.Should().Contain("Great code quality");
    }

    [Fact]
    public async Task Handle_WithNonExistingAuthor_ReturnsFailure()
    {
        // Arrange
        var command = new CreateFeedbackCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Great work on the project implementation.",
            false,
            null
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync(command.AuthorId, It.IsAny<CancellationToken>()))
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

        var command = new CreateFeedbackCommand(
            author.Id,
            Guid.NewGuid(),
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Excellent attention to detail in code reviews.",
            false,
            null
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(command.RecipientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Recipient user not found");
    }

    [Fact]
    public async Task Handle_WithDifferentTeams_ReturnsFailure()
    {
        // Arrange
        var teamId1 = Guid.NewGuid();
        var teamId2 = Guid.NewGuid();
        var author = new User("author@test.com", "hash", "Author", UserRole.Member, teamId1);
        var recipient = new User("recipient@test.com", "hash", "Recipient", UserRole.Member, teamId2);

        var command = new CreateFeedbackCommand(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.Teamwork,
            "Great collaboration on the recent project deliverables.",
            false,
            null
        );

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
    public async Task Handle_WithNoActiveSprint_ReturnsFailure()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var author = new User("author@test.com", "hash", "Author", UserRole.Member, teamId);
        var recipient = new User("recipient@test.com", "hash", "Recipient", UserRole.Member, teamId);

        var command = new CreateFeedbackCommand(
            author.Id,
            recipient.Id,
            FeedbackType.Constructive,
            FeedbackCategory.Communication,
            "Consider improving communication in stand-ups.",
            false,
            null
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(recipient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipient);
        _sprintRepositoryMock.Setup(x => x.GetActualSprintAsync(teamId))
            .ReturnsAsync((Sprint?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("There is no sprint going");
    }

    [Fact]
    public async Task Handle_WithInvalidFeeling_ReturnsFailure()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var author = new User("author@test.com", "hash", "Author", UserRole.Member, teamId);
        var recipient = new User("recipient@test.com", "hash", "Recipient", UserRole.Member, teamId);
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);
        var feelingId = Guid.NewGuid();

        var command = new CreateFeedbackCommand(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.Leadership,
            "Demonstrated excellent leadership skills.",
            false,
            feelingId
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(recipient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipient);
        _sprintRepositoryMock.Setup(x => x.GetActualSprintAsync(teamId))
            .ReturnsAsync(sprint);
        _feelingRepositoryMock.Setup(x => x.GetByIdAsync(feelingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Feeling?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Feeling could not be found");
    }

    [Fact]
    public async Task Handle_WithValidFeeling_IncludesFeelingInResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var author = new User("author@test.com", "hash", "Author", UserRole.Member, teamId);
        var recipient = new User("recipient@test.com", "hash", "Recipient", UserRole.Member, teamId);
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);
        var feeling = new Feeling("Happy");

        var command = new CreateFeedbackCommand(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.ProblemSolving,
            "Excellent problem-solving skills demonstrated.",
            false,
            feeling.Id
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(recipient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipient);
        _sprintRepositoryMock.Setup(x => x.GetActualSprintAsync(teamId))
            .ReturnsAsync(sprint);
        _feelingRepositoryMock.Setup(x => x.GetByIdAsync(feeling.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feeling);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Feeling.Should().Be("Happy");
    }

    [Fact]
    public async Task Handle_CallsSaveChanges()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var author = new User("author@test.com", "hash", "Author", UserRole.Member, teamId);
        var recipient = new User("recipient@test.com", "hash", "Recipient", UserRole.Member, teamId);
        var sprint = new Sprint("Sprint 1", "Description", DateTime.Now, DateTime.Now.AddDays(14), teamId);

        var command = new CreateFeedbackCommand(
            author.Id,
            recipient.Id,
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Excellent code quality and attention to detail.",
            false,
            null
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(recipient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipient);
        _sprintRepositoryMock.Setup(x => x.GetActualSprintAsync(teamId))
            .ReturnsAsync(sprint);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}