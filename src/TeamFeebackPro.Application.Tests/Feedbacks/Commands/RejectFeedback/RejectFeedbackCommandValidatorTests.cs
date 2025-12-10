using FluentAssertions;
using FluentValidation.TestHelper;
using TeamFeedbackPro.Application.Feedbacks.Commands.RejectFeedback;

namespace TeamFeedbackPro.Application.Tests.Feedbacks.Commands.RejectFeedback;

public class RejectFeedbackCommandValidatorTests
{
    private readonly RejectFeedbackCommandValidator _validator;

    public RejectFeedbackCommandValidatorTests()
    {
        _validator = new RejectFeedbackCommandValidator();
    }

    [Fact]
    public void Validator_WithValidCommand_PassesValidation()
    {
        // Arrange
        var command = new RejectFeedbackCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "This feedback needs more specific examples to be actionable."
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_WithEmptyFeedbackId_FailsValidation()
    {
        // Arrange
        var command = new RejectFeedbackCommand(
            Guid.Empty,
            Guid.NewGuid(),
            "This is a valid review with sufficient length."
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FeedbackId)
            .WithErrorMessage("Feedback Id is required");
    }

    [Fact]
    public void Validator_WithEmptyManagerId_FailsValidation()
    {
        // Arrange
        var command = new RejectFeedbackCommand(
            Guid.NewGuid(),
            Guid.Empty,
            "This is a valid review with sufficient length."
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ManagerId)
            .WithErrorMessage("Manager Id is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validator_WithEmptyReview_FailsValidation(string review)
    {
        // Arrange
        var command = new RejectFeedbackCommand(Guid.NewGuid(), Guid.NewGuid(), review);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Review)
            .WithErrorMessage("Review is required");
    }

    [Fact]
    public void Validator_WithTooShortReview_FailsValidation()
    {
        // Arrange
        var command = new RejectFeedbackCommand(Guid.NewGuid(), Guid.NewGuid(), "Too short");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Review)
            .WithErrorMessage("Review must be at least 20 characters");
    }

    [Fact]
    public void Validator_WithTooLongReview_FailsValidation()
    {
        // Arrange
        var command = new RejectFeedbackCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new string('a', 2001)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Review)
            .WithErrorMessage("Review must not exceed 2000 characters");
    }

    [Fact]
    public void Validator_WithExactly20Characters_PassesValidation()
    {
        // Arrange
        var command = new RejectFeedbackCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "12345678901234567890" // Exactly 20 chars
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Review);
    }

    [Fact]
    public void Validator_WithExactly2000Characters_PassesValidation()
    {
        // Arrange
        var command = new RejectFeedbackCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new string('a', 2000) // Exactly 2000 chars
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Review);
    }
}