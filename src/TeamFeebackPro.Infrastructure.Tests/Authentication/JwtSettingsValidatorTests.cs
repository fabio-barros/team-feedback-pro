using FluentAssertions;
using TeamFeedBackPro.Infrastructure.Authentication;

namespace TeamFeedbackPro.Infrastructure.Tests.Authentication;

public class JwtSettingsValidatorTests
{
    private readonly JwtSettingsValidator _validator = new();

    private static JwtSettings CreateValidSettings() => new()
    {
        Secret = "12345678901234567890123456789012", // 32 chars
        Issuer = "issuer",
        Audience = "audience",
        ExpiryDays = 7
    };

    [Fact]
    public void Validate_WithValidSettings_ReturnsSuccess()
    {
        // Arrange
        var settings = CreateValidSettings();

        // Act
        var result = _validator.Validate(null, settings);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNullSettings_ReturnsFailure()
    {
        // Act
        var result = _validator.Validate(null, null);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("JwtSettings is null.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithNullOrWhiteSpaceSecret_ReturnsFailure(string? secret)
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = secret!,
            Issuer = "issuer",
            Audience = "audience",
            ExpiryDays = 7
        };

        // Act
        var result = _validator.Validate(null, settings);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("JwtSettings:Secret must be provided and at least 32 characters long.");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("1234567890123456789012345678901")] // 31 chars
    public void Validate_WithSecretLessThan32Characters_ReturnsFailure(string secret)
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = secret,
            Issuer = "issuer",
            Audience = "audience",
            ExpiryDays = 7
        };

        // Act
        var result = _validator.Validate(null, settings);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("JwtSettings:Secret must be provided and at least 32 characters long.");
    }

    [Fact]
    public void Validate_WithSecretExactly32Characters_ReturnsSuccess()
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = new string('a', 32),
            Issuer = "issuer",
            Audience = "audience",
            ExpiryDays = 7
        };

        // Act
        var result = _validator.Validate(null, settings);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidIssuer_ReturnsFailure(string? issuer)
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = "12345678901234567890123456789012",
            Issuer = issuer!,
            Audience = "audience",
            ExpiryDays = 7
        };

        // Act
        var result = _validator.Validate(null, settings);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("JwtSettings:Issuer is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidAudience_ReturnsFailure(string? audience)
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = "12345678901234567890123456789012",
            Issuer = "issuer",
            Audience = audience!,
            ExpiryDays = 7
        };

        // Act
        var result = _validator.Validate(null, settings);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("JwtSettings:Audience is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithExpiryDaysLessThanOrEqualToZero_ReturnsFailure(int expiryDays)
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = "12345678901234567890123456789012",
            Issuer = "issuer",
            Audience = "audience",
            ExpiryDays = expiryDays
        };

        // Act
        var result = _validator.Validate(null, settings);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("JwtSettings:ExpiryDays must be greater than zero.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(365)]
    public void Validate_WithValidExpiryDays_ReturnsSuccess(int expiryDays)
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = "12345678901234567890123456789012",
            Issuer = "issuer",
            Audience = "audience",
            ExpiryDays = expiryDays
        };

        // Act
        var result = _validator.Validate(null, settings);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithMultipleInvalidFields_ReturnsAllErrors()
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = "short",
            Issuer = "",
            Audience = "   ",
            ExpiryDays = 0
        };

        // Act
        var result = _validator.Validate(null, settings);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("JwtSettings:Secret must be provided and at least 32 characters long.");
        result.FailureMessage.Should().Contain("JwtSettings:Issuer is required.");
        result.FailureMessage.Should().Contain("JwtSettings:Audience is required.");
        result.FailureMessage.Should().Contain("JwtSettings:ExpiryDays must be greater than zero.");
    }

    [Fact]
    public void Validate_WithAllFieldsNull_ReturnsAllErrors()
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = null!,
            Issuer = null!,
            Audience = null!,
            ExpiryDays = 0
        };

        // Act
        var result = _validator.Validate(null, settings);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("JwtSettings:Secret must be provided and at least 32 characters long.");
        result.FailureMessage.Should().Contain("JwtSettings:Issuer is required.");
        result.FailureMessage.Should().Contain("JwtSettings:Audience is required.");
        result.FailureMessage.Should().Contain("JwtSettings:ExpiryDays must be greater than zero.");
    }

    [Fact]
    public void Validate_WithPartiallyInvalidSettings_ReturnsRelevantErrors()
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = "short",
            Issuer = "issuer",
            Audience = "audience",
            ExpiryDays = -5
        };

        // Act
        var result = _validator.Validate(null, settings);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("JwtSettings:Secret must be provided and at least 32 characters long.");
        result.FailureMessage.Should().Contain("JwtSettings:ExpiryDays must be greater than zero.");
        result.FailureMessage.Should().NotContain("Issuer");
        result.FailureMessage.Should().NotContain("Audience");
    }

    [Fact]
    public void Validate_WithNameParameter_StillValidates()
    {
        // Arrange
        var settings = CreateValidSettings();

        // Act
        var result = _validator.Validate("CustomName", settings);

        // Assert
        result.Succeeded.Should().BeTrue();
    }
}