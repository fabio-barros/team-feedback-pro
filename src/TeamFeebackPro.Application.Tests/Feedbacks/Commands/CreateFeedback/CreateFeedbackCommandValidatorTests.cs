using FluentAssertions;
using FluentValidation.TestHelper;
using TeamFeedbackPro.Application.Feedbacks.Commands.CreateFeedback;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Feedbacks.Commands.CreateFeedback;

public class CreateFeedbackCommandValidatorTests
{
    private readonly CreateFeedbackCommandValidator _validator;

    public CreateFeedbackCommandValidatorTests()
    {
        _validator = new CreateFeedbackCommandValidator();
    }

    [Fact]
    public void Validator_WithValidCommand_PassesValidation()
    {
        // Arrange
        var command = new CreateFeedbackCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "This is a valid feedback content with more than twenty characters.",
            false,
            null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_WithEmptyAuthorId_FailsValidation()
    {
        // Arrange
        var command = new CreateFeedbackCommand(
            Guid.Empty,
            Guid.NewGuid(),
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "This is valid feedback content.",
            false,
            null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AuthorId)
            .WithErrorMessage("Author ID is required");
    }

    [Fact]
    public void Validator_WithEmptyRecipientId_FailsValidation()
    {
        // Arrange
        var command = new CreateFeedbackCommand(
            Guid.NewGuid(),
            Guid.Empty,
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "This is valid feedback content.",
            false,
            null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RecipientId)
            .WithErrorMessage("Recipient ID is required");
    }

    [Fact]
    public void Validator_WithSameAuthorAndRecipient_FailsValidation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateFeedbackCommand(
            userId,
            userId,
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "This is valid feedback content.",
            false,
            null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RecipientId)
            .WithErrorMessage("Cannot send feedback to yourself");
    }

    [Fact]
    public void Validator_WithInvalidType_FailsValidation()
    {
        // Arrange
        var command = new CreateFeedbackCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            (FeedbackType)999,
            FeedbackCategory.CodeQuality,
            "This is valid feedback content.",
            false,
            null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type)
            .WithErrorMessage("Invalid feedback type");
    }

    [Fact]
    public void Validator_WithInvalidCategory_FailsValidation()
    {
        // Arrange
        var command = new CreateFeedbackCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            FeedbackType.Positive,
            (FeedbackCategory)999,
            "This is valid feedback content.",
            false,
            null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Category)
            .WithErrorMessage("Invalid feedback category");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validator_WithEmptyContent_FailsValidation(string content)
    {
        // Arrange
        var command = new CreateFeedbackCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            content,
            false,
            null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Content is required");
    }

    [Fact]
    public void Validator_WithTooShortContent_FailsValidation()
    {
        // Arrange
        var command = new CreateFeedbackCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            "Too short",
            false,
            null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Content must be at least 20 characters");
    }

    [Fact]
    public void Validator_WithTooLongContent_FailsValidation()
    {
        // Arrange
        var command = new CreateFeedbackCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            FeedbackType.Positive,
            FeedbackCategory.CodeQuality,
            new string('a', 2001),
            false,
            null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Content must not exceed 2000 characters");
    }
}