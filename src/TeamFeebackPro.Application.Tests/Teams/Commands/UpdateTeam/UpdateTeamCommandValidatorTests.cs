using FluentAssertions;
using FluentValidation.TestHelper;
using TeamFeedbackPro.Application.Teams.Commands.UpdateTeam;

namespace TeamFeedbackPro.Application.Tests.Teams.Commands.UpdateTeam;

public class UpdateTeamCommandValidatorTests
{
    private readonly UpdateTeamCommandValidator _validator;

    public UpdateTeamCommandValidatorTests()
    {
        _validator = new UpdateTeamCommandValidator();
    }

    [Fact]
    public void Validator_WithValidCommand_PassesValidation()
    {
        // Arrange
        var command = new UpdateTeamCommand(Guid.NewGuid(), "Engineering", Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_WithEmptyTeamId_FailsValidation()
    {
        // Arrange
        var command = new UpdateTeamCommand(Guid.Empty, "Engineering", null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Team ID is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validator_WithEmptyName_FailsValidation(string name)
    {
        // Arrange
        var command = new UpdateTeamCommand(Guid.NewGuid(), name, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Team name is required");
    }

    [Fact]
    public void Validator_WithWhitespaceName_FailsValidation()
    {
        // Arrange
        var command = new UpdateTeamCommand(Guid.NewGuid(), "   ", null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Team name cannot be only whitespace");
    }

    [Fact]
    public void Validator_WithTooLongName_FailsValidation()
    {
        // Arrange
        var longName = new string('a', 201);
        var command = new UpdateTeamCommand(Guid.NewGuid(), longName, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Team name must not exceed 200 characters");
    }

    [Fact]
    public void Validator_WithExactly200Characters_PassesValidation()
    {
        // Arrange
        var name = new string('a', 200);
        var command = new UpdateTeamCommand(Guid.NewGuid(), name, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}