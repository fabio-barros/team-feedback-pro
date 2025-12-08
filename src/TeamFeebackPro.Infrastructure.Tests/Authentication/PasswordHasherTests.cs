using FluentAssertions;
using TeamFeedBackPro.Infrastructure.Authentication;

namespace TeamFeedbackPro.Infrastructure.Tests.Authentication;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_ShouldReturnDifferentHashForSamePassword()
    {
        // Arrange
        var hasher = new PasswordHasher();
        var password = "SecurePassword123!";

        // Act
        var hash1 = hasher.HashPassword(password);
        var hash2 = hasher.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2);
        hash1.Should().NotBeNullOrWhiteSpace();
        hash2.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrueForCorrectPassword()
    {
        // Arrange
        var hasher = new PasswordHasher();
        var password = "SecurePassword123!";
        var hash = hasher.HashPassword(password);

        // Act
        var result = hasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalseForIncorrectPassword()
    {
        // Arrange
        var hasher = new PasswordHasher();
        var password = "SecurePassword123!";
        var wrongPassword = "WrongPassword!";
        var hash = hasher.HashPassword(password);

        // Act
        var result = hasher.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }
}