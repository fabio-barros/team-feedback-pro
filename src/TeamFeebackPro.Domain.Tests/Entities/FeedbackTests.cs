using FluentAssertions;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeebackPro.Domain.Tests.Entities;

public class FeedbackTests
{
    [Fact]
    public void Feedback_Constructor_ShouldCreateValidFeedback()
    {
        // Arrange
        const string content = "Great job on the project! Your attention to detail was excellent.";
        var authorId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();
        const FeedbackType type = FeedbackType.Positive;
        const FeedbackCategory category = FeedbackCategory.Communication;

        // Act
        var sut = new Feedback(authorId, recipientId, type, category, content, false, teamId, sprintId);

        // Assert
        sut.Content.Should().Be(content);
        sut.AuthorId.Should().Be(authorId);
        sut.RecipientId.Should().Be(recipientId);
        sut.Type.Should().Be(type);
        sut.Category.Should().Be(category);
        sut.IsAnonymous.Should().BeFalse();
        sut.Status.Should().Be(FeedbackStatus.Pending);
        sut.TeamId.Should().Be(teamId);
        sut.SprintId.Should().Be(sprintId);
        sut.FeelingId.Should().BeNull();
        sut.ReviewedBy.Should().BeNull();
        sut.ReviewedAt.Should().BeNull();
        sut.ReviewNotes.Should().BeNull();
        sut.Id.Should().NotBeEmpty();
        sut.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Feedback_Constructor_AsAnonymous_ShouldSetIsAnonymousTrue()
    {
        // Arrange
        const string content = "This is anonymous feedback for improvement and growth.";
        var authorId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();

        // Act
        var sut = new Feedback(authorId, recipientId, FeedbackType.Constructive, FeedbackCategory.CodeQuality, content, true, teamId, sprintId);

        // Assert
        sut.IsAnonymous.Should().BeTrue();
    }

    [Fact]
    public void Feedback_Constructor_WithFeelingId_ShouldSetFeelingId()
    {
        // Arrange
        var feelingId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();
        var teamId = Guid.NewGuid();

        // Act
        var sut = CreateTestFeedback(teamId: teamId, sprintId: sprintId, feelingId: feelingId);

        // Assert
        sut.FeelingId.Should().Be(feelingId);
    }

    [Fact]
    public void Feedback_Constructor_WithoutFeelingId_ShouldHaveNullFeelingId()
    {
        // Arrange & Act
        var sut = CreateTestFeedback();

        // Assert
        sut.FeelingId.Should().BeNull();
    }

    [Fact]
    public void Feedback_Constructor_ShouldTrimContent()
    {
        // Arrange
        const string content = "  This is feedback with spaces at both ends that should be trimmed properly.  ";
        var teamId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();

        // Act
        var sut = CreateTestFeedback(content: content, teamId: teamId, sprintId: sprintId);

        // Assert
        sut.Content.Should().Be(content.Trim());
    }

    [Fact]
    public void Feedback_Constructor_WithSameAuthorAndRecipient_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();

        // Act
        var act = () => new Feedback(userId, userId, FeedbackType.Positive, FeedbackCategory.Communication, "Valid content with more than twenty characters", false, teamId, sprintId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Cannot send feedback to yourself");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Feedback_Constructor_WithInvalidContent_ShouldThrowArgumentException(string invalidContent)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();

        // Act
        var act = () => CreateTestFeedback(content: invalidContent, teamId: teamId, sprintId: sprintId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Content cannot be empty*")
            .And.ParamName.Should().Be("content");
    }

    [Theory]
    [InlineData("Short")]
    [InlineData("Less than twenty!")]
    [InlineData("Exactly 19 chars!!")]
    public void Feedback_Constructor_WithTooShortContent_ShouldThrowArgumentException(string shortContent)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();

        // Act
        var act = () => CreateTestFeedback(content: shortContent, teamId: teamId, sprintId: sprintId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Content must be at least 20 characters*")
            .And.ParamName.Should().Be("content");
    }

    [Fact]
    public void Feedback_Constructor_WithTooLongContent_ShouldThrowArgumentException()
    {
        // Arrange
        var longContent = new string('a', 2001);
        var teamId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();

        // Act
        var act = () => CreateTestFeedback(content: longContent, teamId: teamId, sprintId: sprintId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Content must not exceed 2000 characters*")
            .And.ParamName.Should().Be("content");
    }

    [Fact]
    public void Feedback_Constructor_WithExactly20Characters_ShouldSucceed()
    {
        // Arrange
        const string content = "12345678901234567890"; // Exactly 20 chars
        var teamId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();

        // Act
        var sut = CreateTestFeedback(content: content, teamId: teamId, sprintId: sprintId);

        // Assert
        sut.Content.Should().Be(content);
    }

    [Fact]
    public void Feedback_Constructor_WithExactly2000Characters_ShouldSucceed()
    {
        // Arrange
        var content = new string('a', 2000); // Exactly 2000 chars
        var teamId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();

        // Act
        var sut = CreateTestFeedback(content: content, teamId: teamId, sprintId: sprintId);

        // Assert
        sut.Content.Should().HaveLength(2000);
    }

    [Fact]
    public void Feedback_Approve_ShouldSetStatusToApprovedAndUpdateMetadata()
    {
        // Arrange
        var sut = CreateTestFeedback();
        var reviewerId = Guid.NewGuid();
        const string notes = "Looks good!";
        var originalUpdatedAt = sut.UpdatedAt;
        Thread.Sleep(10);

        // Act
        sut.Approve(reviewerId, notes);

        // Assert
        sut.Status.Should().Be(FeedbackStatus.Approved);
        sut.ReviewedBy.Should().Be(reviewerId);
        sut.ReviewedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        sut.ReviewNotes.Should().Be(notes);
        sut.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Feedback_Approve_WithoutNotes_ShouldSucceed()
    {
        // Arrange
        var sut = CreateTestFeedback();
        var reviewerId = Guid.NewGuid();

        // Act
        sut.Approve(reviewerId);

        // Assert
        sut.Status.Should().Be(FeedbackStatus.Approved);
        sut.ReviewedBy.Should().Be(reviewerId);
        sut.ReviewNotes.Should().BeNull();
    }

    [Fact]
    public void Feedback_Approve_WhenNotPending_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sut = CreateTestFeedback();
        var reviewerId = Guid.NewGuid();
        sut.Approve(reviewerId);

        // Act
        var act = () => sut.Approve(reviewerId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending feedbacks can be approved");
    }

    [Fact]
    public void Feedback_Reject_ShouldSetStatusToRejectedAndUpdateMetadata()
    {
        // Arrange
        var sut = CreateTestFeedback();
        var reviewerId = Guid.NewGuid();
        const string notes = "Content needs improvement";
        var originalUpdatedAt = sut.UpdatedAt;
        Thread.Sleep(10);

        // Act
        sut.Reject(reviewerId, notes);

        // Assert
        sut.Status.Should().Be(FeedbackStatus.Rejected);
        sut.ReviewedBy.Should().Be(reviewerId);
        sut.ReviewedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        sut.ReviewNotes.Should().Be(notes);
        sut.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Feedback_Reject_WithoutNotes_ShouldThrowArgumentException(string invalidNotes)
    {
        // Arrange
        var sut = CreateTestFeedback();
        var reviewerId = Guid.NewGuid();

        // Act
        var act = () => sut.Reject(reviewerId, invalidNotes);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Rejection notes are required*")
            .And.ParamName.Should().Be("notes");
    }

    [Fact]
    public void Feedback_Reject_WhenNotPending_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sut = CreateTestFeedback();
        var reviewerId = Guid.NewGuid();
        sut.Reject(reviewerId, "First rejection");

        // Act
        var act = () => sut.Reject(reviewerId, "Second rejection");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending feedbacks can be rejected");
    }

    [Theory]
    [InlineData(FeedbackType.Positive)]
    [InlineData(FeedbackType.Constructive)]
    [InlineData(FeedbackType.Critical)]
    public void Feedback_Constructor_ShouldAcceptAllValidTypes(FeedbackType type)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();

        // Act
        var sut = CreateTestFeedback(type: type, teamId: teamId, sprintId: sprintId);

        // Assert
        sut.Type.Should().Be(type);
    }

    [Theory]
    [InlineData(FeedbackCategory.Teamwork)]
    [InlineData(FeedbackCategory.CodeQuality)]
    [InlineData(FeedbackCategory.Communication)]
    [InlineData(FeedbackCategory.ProblemSolving)]
    [InlineData(FeedbackCategory.Leadership)]
    [InlineData(FeedbackCategory.Other)]
    public void Feedback_Constructor_ShouldAcceptAllValidCategories(FeedbackCategory category)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();

        // Act
        var sut = CreateTestFeedback(category: category, teamId: teamId, sprintId: sprintId);

        // Assert
        sut.Category.Should().Be(category);
    }

    [Fact]
    public void Feedback_DefaultStatus_ShouldBePending()
    {
        // Arrange & Act
        var sut = CreateTestFeedback();

        // Assert
        sut.Status.Should().Be(FeedbackStatus.Pending);
    }

    [Fact]
    public void Feedback_CreatedAt_ShouldBeSetOnConstruction()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var sut = CreateTestFeedback();

        // Assert
        var after = DateTime.UtcNow;
        sut.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Feedback_UpdatedAt_ShouldBeSetOnConstruction()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var sut = CreateTestFeedback();

        // Assert
        var after = DateTime.UtcNow;
        sut.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Feedback_NavigationProperties_ShouldBeNullOnCreation()
    {
        // Arrange & Act
        var sut = CreateTestFeedback();

        // Assert
        sut.Author.Should().BeNull();
        sut.Recipient.Should().BeNull();
        sut.Reviewer.Should().BeNull();
        sut.Team.Should().BeNull();
        sut.Sprint.Should().BeNull();
    }

    [Fact]
    public void Feedback_Approve_ThenReject_ShouldNotAllowReject()
    {
        // Arrange
        var sut = CreateTestFeedback();
        var reviewerId = Guid.NewGuid();
        sut.Approve(reviewerId);

        // Act
        var act = () => sut.Reject(reviewerId, "Trying to reject");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending feedbacks can be rejected");
    }

    [Fact]
    public void Feedback_Reject_ThenApprove_ShouldNotAllowApprove()
    {
        // Arrange
        var sut = CreateTestFeedback();
        var reviewerId = Guid.NewGuid();
        sut.Reject(reviewerId, "Rejected");

        // Act
        var act = () => sut.Approve(reviewerId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending feedbacks can be approved");
    }

    private static Feedback CreateTestFeedback(
        string content = "This is a valid feedback message with enough characters to pass validation rules.",
        FeedbackType type = FeedbackType.Positive,
        FeedbackCategory category = FeedbackCategory.Communication,
        Guid? teamId = null,
        Guid? sprintId = null,
        Guid? feelingId = null)
    {
        return new Feedback(
            Guid.NewGuid(),
            Guid.NewGuid(),
            type,
            category,
            content,
            false,
            teamId ?? Guid.NewGuid(),
            sprintId ?? Guid.NewGuid(),
            feelingId
        );
    }
}