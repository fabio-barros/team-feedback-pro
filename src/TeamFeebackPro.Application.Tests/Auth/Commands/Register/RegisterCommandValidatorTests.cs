using FluentAssertions;
using FluentValidation.TestHelper;
using TeamFeedbackPro.Application.Auth.Commands.Register;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Auth.Commands.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }

    [Fact]
    public void Validator_WithValidCommand_PassesValidation()
    {
        // Arrange
        var command = new RegisterCommand(
            "test@example.com",
            "Password123",
            "Test User",
            UserRole.Member,
            Guid.NewGuid()
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
    public void Validator_WithEmptyEmail_FailsValidation(string email)
    {
        // Arrange
        var command = new RegisterCommand(email, "Password123", "Test User", UserRole.Member, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test")]
    public void Validator_WithInvalidEmailFormat_FailsValidation(string email)
    {
        // Arrange
        var command = new RegisterCommand(email, "Password123", "Test User", UserRole.Member, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format");
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validator_WithEmptyPassword_FailsValidation(string password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password, "Test User", UserRole.Member, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Fact]
    public void Validator_WithShortPassword_FailsValidation()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Pass1", "Test User", UserRole.Member, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters");
    }

    [Fact]
    public void Validator_WithPasswordMissingUppercase_FailsValidation()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "password123", "Test User", UserRole.Member, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter");
    }

    [Fact]
    public void Validator_WithPasswordMissingLowercase_FailsValidation()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "PASSWORD123", "Test User", UserRole.Member, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one lowercase letter");
    }

    [Fact]
    public void Validator_WithPasswordMissingNumber_FailsValidation()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "PasswordOnly", "Test User", UserRole.Member, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one number");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validator_WithEmptyName_FailsValidation(string name)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Password123", name, UserRole.Member, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Validator_WithTooLongName_FailsValidation()
    {
        // Arrange
        var longName = new string('a', 201);
        var command = new RegisterCommand("test@example.com", "Password123", longName, UserRole.Member, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name must not exceed 200 characters");
    }

    [Fact]
    public void Validator_WithInvalidRole_FailsValidation()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Password123", "Test User", (UserRole)999, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Role)
            .WithErrorMessage("Invalid role");
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Manager)]
    [InlineData(UserRole.Member)]
    public void Validator_WithValidRole_PassesValidation(UserRole role)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Password123", "Test User", role, null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }
}