using FluentAssertions;
using FluentValidation.TestHelper;
using TeamFeedbackPro.Application.Feedbacks.Commands.ApproveFeedback;

namespace TeamFeedbackPro.Application.Tests.Feedbacks.Commands.ApproveFeedback;

public class ApproveFeedbackCommandValidatorTests
{
    private readonly ApproveFeedbackCommandValidator _validator;

    public ApproveFeedbackCommandValidatorTests()
    {
        _validator = new ApproveFeedbackCommandValidator();
    }

    [Fact]
    public void Validator_WithValidCommand_PassesValidation()
    {
        // Arrange
        var command = new ApproveFeedbackCommand(Guid.NewGuid(), Guid.NewGuid(), "Approved");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_WithEmptyFeedbackId_FailsValidation()
    {
        // Arrange
        var command = new ApproveFeedbackCommand(Guid.Empty, Guid.NewGuid(), null);

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
        var command = new ApproveFeedbackCommand(Guid.NewGuid(), Guid.Empty, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ManagerId)
            .WithErrorMessage("Manager Id is required");
    }

    [Fact]
    public void Validator_WithNullReview_PassesValidation()
    {
        // Arrange
        var command = new ApproveFeedbackCommand(Guid.NewGuid(), Guid.NewGuid(), null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Review);
    }
}