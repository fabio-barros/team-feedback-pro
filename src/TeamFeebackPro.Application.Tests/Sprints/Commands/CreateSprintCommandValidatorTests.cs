using FluentAssertions;
using FluentValidation.TestHelper;
using TeamFeedbackPro.Application.Sprints.Commands.CreateSprint;

namespace TeamFeedbackPro.Application.Tests.Sprints.Commands.CreateSprint;

public class CreateSprintCommandValidatorTests
{
    private readonly CreateSprintCommandValidator _validator;

    public CreateSprintCommandValidatorTests()
    {
        _validator = new CreateSprintCommandValidator();
    }

    [Fact]
    public void Validator_WithValidCommand_PassesValidation()
    {
        // Arrange
        var command = new CreateSprintCommand(
            Guid.NewGuid(),
            "Sprint 1",
            "This is a valid sprint description with sufficient length.",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validator_WithEmptyName_FailsValidation(string name)
    {
        // Arrange
        var command = new CreateSprintCommand(
            Guid.NewGuid(),
            name,
            "Valid description with more than twenty characters.",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Sprint name is required");
    }


    [Fact]
    public void Validator_WithTooShortDescription_FailsValidation()
    {
        // Arrange
        var command = new CreateSprintCommand(
            Guid.NewGuid(),
            "Sprint 1",
            "Too short",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Content must be at least 20 characters");
    }

    [Fact]
    public void Validator_WithTooLongDescription_FailsValidation()
    {
        // Arrange
        var longDescription = new string('a', 2001);
        var command = new CreateSprintCommand(
            Guid.NewGuid(),
            "Sprint 1",
            longDescription,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Content must not exceed 2000 characters");
    }

    [Fact]
    public void Validator_WithExactly20Characters_PassesValidation()
    {
        // Arrange
        var description = "12345678901234567890"; // Exactly 20 characters
        var command = new CreateSprintCommand(
            Guid.NewGuid(),
            "Sprint 1",
            description,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validator_WithExactly2000Characters_PassesValidation()
    {
        // Arrange
        var description = new string('a', 2000); // Exactly 2000 characters
        var command = new CreateSprintCommand(
            Guid.NewGuid(),
            "Sprint 1",
            description,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validator_WithValidDates_PassesValidation()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 1, 15);
        var command = new CreateSprintCommand(
            Guid.NewGuid(),
            "Sprint 1",
            "Valid sprint description with sufficient character count.",
            startDate,
            endDate
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_WithEmptyDescription_PassesValidation()
    {
        // Arrange
        // Description is optional based on validator rules (only length is validated)
        var command = new CreateSprintCommand(
            Guid.NewGuid(),
            "Sprint 1",
            null!,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        // Should not have description errors since it's not marked as required
        result.Errors.Should().NotContain(e => e.PropertyName == "Description" && e.ErrorMessage.Contains("required"));
    }
}