using FluentAssertions;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void User_Constructor_ShouldCreateValidUser()
    {
        // Arrange
        const string email = "test@example.com";
        const string passwordHash = "hashedPassword123";
        const string name = "Test User";
        const UserRole role = UserRole.Member;
        var teamId = Guid.NewGuid();

        // Act
        var sut = new User(email, passwordHash, name, role, teamId);

        // Assert
        sut.Email.Should().Be(email.ToLowerInvariant());
        sut.PasswordHash.Should().Be(passwordHash);
        sut.Name.Should().Be(name);
        sut.Role.Should().Be(role);
        sut.TeamId.Should().Be(teamId);
        sut.Id.Should().NotBeEmpty();
        sut.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void User_Constructor_WithoutTeamId_ShouldCreateValidUser()
    {
        // Arrange
        const string email = "test@example.com";
        const string passwordHash = "hashedPassword123";
        const string name = "Test User";
        const UserRole role = UserRole.Member;

        // Act
        var sut = new User(email, passwordHash, name, role);

        // Assert
        sut.Email.Should().Be(email.ToLowerInvariant());
        sut.PasswordHash.Should().Be(passwordHash);
        sut.Name.Should().Be(name);
        sut.Role.Should().Be(role);
        sut.TeamId.Should().BeNull();
        sut.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void User_Constructor_ShouldConvertEmailToLowerCase()
    {
        // Arrange
        const string email = "TEST.USER@EXAMPLE.COM";

        // Act
        var sut = new User(email, "hash", "Test", UserRole.Member);

        // Assert
        sut.Email.Should().Be("test.user@example.com");
    }

    [Fact]
    public void User_Constructor_ShouldTrimEmailAndName()
    {
        // Arrange
        const string email = "  test@example.com  ";
        const string name = "  Test User  ";

        // Act
        var sut = new User(email, "hash", name, UserRole.Member);

        // Assert
        sut.Email.Should().Be("test@example.com");
        sut.Name.Should().Be("Test User");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void User_Constructor_WithInvalidEmail_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act
        var act = () => new User(invalidEmail, "hash", "Test", UserRole.Member);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Email cannot be empty*")
            .And.ParamName.Should().Be("email");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void User_Constructor_WithInvalidPasswordHash_ShouldThrowArgumentException(string invalidPasswordHash)
    {
        // Act
        var act = () => new User("test@example.com", invalidPasswordHash, "Test", UserRole.Member);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Password hash cannot be empty*")
            .And.ParamName.Should().Be("passwordHash");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void User_Constructor_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Act
        var act = () => new User("test@example.com", "hash", invalidName, UserRole.Member);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Name cannot be empty*")
            .And.ParamName.Should().Be("name");
    }

    [Fact]
    public void User_UpdateRole_ShouldUpdateRoleAndTimestamp()
    {
        // Arrange
        var sut = CreateTestUser();
        var originalUpdatedAt = sut.UpdatedAt;
        Thread.Sleep(10); // Ensure timestamp changes

        // Act
        sut.UpdateRole(UserRole.Manager);

        // Assert
        sut.Role.Should().Be(UserRole.Manager);
        sut.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void User_AssignToTeam_ShouldSetTeamIdAndUpdateTimestamp()
    {
        // Arrange
        var sut = CreateTestUser();
        var originalUpdatedAt = sut.UpdatedAt;
        var newTeamId = Guid.NewGuid();
        Thread.Sleep(10);

        // Act
        sut.AssignToTeam(newTeamId);

        // Assert
        sut.TeamId.Should().Be(newTeamId);
        sut.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void User_UpdatePassword_ShouldUpdatePasswordHashAndTimestamp()
    {
        // Arrange
        var sut = CreateTestUser();
        var originalUpdatedAt = sut.UpdatedAt;
        const string newPasswordHash = "newHashedPassword456";
        Thread.Sleep(10);

        // Act
        sut.UpdatePassword(newPasswordHash);

        // Assert
        sut.PasswordHash.Should().Be(newPasswordHash);
        sut.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void User_UpdatePassword_WithInvalidPasswordHash_ShouldThrowArgumentException(string invalidPasswordHash)
    {
        // Arrange
        var sut = CreateTestUser();

        // Act
        var act = () => sut.UpdatePassword(invalidPasswordHash);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Password hash cannot be empty*")
            .And.ParamName.Should().Be("newPasswordHash");
    }

    [Fact]
    public void User_UpdateName_ShouldUpdateNameAndTimestamp()
    {
        // Arrange
        var sut = CreateTestUser();
        var originalUpdatedAt = sut.UpdatedAt;
        const string newName = "Updated Name";
        Thread.Sleep(10);

        // Act
        sut.UpdateName(newName);

        // Assert
        sut.Name.Should().Be(newName);
        sut.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void User_UpdateName_ShouldTrimName()
    {
        // Arrange
        var sut = CreateTestUser();
        const string newName = "  Updated Name  ";

        // Act
        sut.UpdateName(newName);

        // Assert
        sut.Name.Should().Be("Updated Name");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void User_UpdateName_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange
        var sut = CreateTestUser();

        // Act
        var act = () => sut.UpdateName(invalidName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Name cannot be empty*")
            .And.ParamName.Should().Be("newName");
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Manager)]
    [InlineData(UserRole.Member)]
    public void User_Constructor_ShouldAcceptAllValidRoles(UserRole role)
    {
        // Act
        var sut = new User("test@example.com", "hash", "Test", role);

        // Assert
        sut.Role.Should().Be(role);
    }

    [Fact]
    public void User_AssignToTeam_MultipleTime_ShouldUpdateTeamId()
    {
        // Arrange
        var sut = CreateTestUser();
        var firstTeamId = Guid.NewGuid();
        var secondTeamId = Guid.NewGuid();

        // Act
        sut.AssignToTeam(firstTeamId);
        sut.AssignToTeam(secondTeamId);

        // Assert
        sut.TeamId.Should().Be(secondTeamId);
    }

    [Fact]
    public void User_UpdateRole_FromMemberToAdmin_ShouldSucceed()
    {
        // Arrange
        var sut = new User("test@example.com", "hash", "Test", UserRole.Member);

        // Act
        sut.UpdateRole(UserRole.Admin);

        // Assert
        sut.Role.Should().Be(UserRole.Admin);
    }

    [Fact]
    public void User_CreatedAt_ShouldBeSetOnConstruction()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var sut = CreateTestUser();

        // Assert
        var after = DateTime.UtcNow;
        sut.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void User_UpdatedAt_ShouldBeSetOnConstruction()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var sut = CreateTestUser();

        // Assert
        var after = DateTime.UtcNow;
        sut.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    private static User CreateTestUser()
    {
        return new User(
            "test@example.com",
            "hashedPassword",
            "Test User",
            UserRole.Member,
            Guid.NewGuid()
        );
    }
}