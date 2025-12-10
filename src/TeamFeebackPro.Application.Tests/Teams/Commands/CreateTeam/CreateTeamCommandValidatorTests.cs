using FluentAssertions;
using FluentValidation.TestHelper;
using TeamFeedbackPro.Application.Teams.Commands.CreateTeam;

namespace TeamFeedbackPro.Application.Tests.Teams.Commands.CreateTeam;

public class CreateTeamCommandValidatorTests
{
    private readonly CreateTeamCommandValidator _validator;

    public CreateTeamCommandValidatorTests()
    {
        _validator = new CreateTeamCommandValidator();
    }

    [Fact]
    public void Validator_WithValidCommand_PassesValidation()
    {
        // Arrange
        var command = new CreateTeamCommand("Engineering", Guid.NewGuid());

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
        var command = new CreateTeamCommand(name, null);

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
        var command = new CreateTeamCommand("   ", null);

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
        var command = new CreateTeamCommand(longName, null);

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
        var command = new CreateTeamCommand(name, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validator_WithNullManagerId_PassesValidation()
    {
        // Arrange
        var command = new CreateTeamCommand("Engineering", null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}