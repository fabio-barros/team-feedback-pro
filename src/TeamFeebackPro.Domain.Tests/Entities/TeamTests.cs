using FluentAssertions;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Domain.Tests.Entities;

public class TeamTests
{
    [Fact]
    public void Team_Constructor_ShouldCreateValidTeam()
    {
        // Arrange
        const string name = "Engineering Team";
        var managerId = Guid.NewGuid();

        // Act
        var sut = new Team(name, managerId);

        // Assert
        sut.Name.Should().Be(name);
        sut.ManagerId.Should().Be(managerId);
        sut.Id.Should().NotBeEmpty();
        sut.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Team_Constructor_WithoutManager_ShouldCreateValidTeam()
    {
        // Arrange
        const string name = "Engineering Team";

        // Act
        var sut = new Team(name);

        // Assert
        sut.Name.Should().Be(name);
        sut.ManagerId.Should().BeNull();
        sut.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Team_Constructor_ShouldTrimName()
    {
        // Arrange
        const string name = "  Engineering Team  ";

        // Act
        var sut = new Team(name);

        // Assert
        sut.Name.Should().Be("Engineering Team");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Team_Constructor_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Act
        var act = () => new Team(invalidName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Team name cannot be empty*")
            .And.ParamName.Should().Be("name");
    }

    [Fact]
    public void Team_UpdateName_ShouldUpdateNameAndTimestamp()
    {
        // Arrange
        var sut = CreateTestTeam();
        var originalUpdatedAt = sut.UpdatedAt;
        const string newName = "Product Team";
        Thread.Sleep(10);

        // Act
        sut.UpdateName(newName);

        // Assert
        sut.Name.Should().Be(newName);
        sut.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Team_UpdateName_ShouldTrimName()
    {
        // Arrange
        var sut = CreateTestTeam();
        const string newName = "  Product Team  ";

        // Act
        sut.UpdateName(newName);

        // Assert
        sut.Name.Should().Be("Product Team");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Team_UpdateName_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange
        var sut = CreateTestTeam();

        // Act
        var act = () => sut.UpdateName(invalidName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Team name cannot be empty*")
            .And.ParamName.Should().Be("newName");
    }

    [Fact]
    public void Team_AssignManager_ShouldSetManagerIdAndUpdateTimestamp()
    {
        // Arrange
        var sut = CreateTestTeam();
        var originalUpdatedAt = sut.UpdatedAt;
        var newManagerId = Guid.NewGuid();
        Thread.Sleep(10);

        // Act
        sut.AssignManager(newManagerId);

        // Assert
        sut.ManagerId.Should().Be(newManagerId);
        sut.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Team_AssignManager_MultipleTimes_ShouldUpdateManagerId()
    {
        // Arrange
        var sut = CreateTestTeam();
        var firstManagerId = Guid.NewGuid();
        var secondManagerId = Guid.NewGuid();

        // Act
        sut.AssignManager(firstManagerId);
        sut.AssignManager(secondManagerId);

        // Assert
        sut.ManagerId.Should().Be(secondManagerId);
    }

    [Fact]
    public void Team_AssignManager_ToTeamWithoutManager_ShouldSetManagerId()
    {
        // Arrange
        var sut = new Team("Test Team");
        var managerId = Guid.NewGuid();

        // Act
        sut.AssignManager(managerId);

        // Assert
        sut.ManagerId.Should().Be(managerId);
    }

    [Fact]
    public void Team_Members_ShouldBeEmptyOnCreation()
    {
        // Arrange & Act
        var sut = CreateTestTeam();

        // Assert
        sut.Members.Should().BeEmpty();
    }

    [Fact]
    public void Team_Members_ShouldBeReadOnly()
    {
        // Arrange
        var sut = CreateTestTeam();

        // Assert
        sut.Members.Should().BeAssignableTo<IReadOnlyCollection<User>>();
    }

    [Fact]
    public void Team_CreatedAt_ShouldBeSetOnConstruction()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var sut = CreateTestTeam();

        // Assert
        var after = DateTime.UtcNow;
        sut.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Team_UpdatedAt_ShouldBeSetOnConstruction()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var sut = CreateTestTeam();

        // Assert
        var after = DateTime.UtcNow;
        sut.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Team_Constructor_WithManagerId_ShouldSetManagerId()
    {
        // Arrange
        const string name = "Test Team";
        var managerId = Guid.NewGuid();

        // Act
        var sut = new Team(name, managerId);

        // Assert
        sut.ManagerId.Should().Be(managerId);
    }

    [Fact]
    public void Team_Constructor_WithoutManagerId_ShouldHaveNullManagerId()
    {
        // Arrange
        const string name = "Test Team";

        // Act
        var sut = new Team(name);

        // Assert
        sut.ManagerId.Should().BeNull();
    }

    [Fact]
    public void Team_UpdateName_ToSameName_ShouldStillUpdateTimestamp()
    {
        // Arrange
        var sut = CreateTestTeam();
        var originalName = sut.Name;
        var originalUpdatedAt = sut.UpdatedAt;
        Thread.Sleep(10);

        // Act
        sut.UpdateName(originalName);

        // Assert
        sut.Name.Should().Be(originalName);
        sut.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    private static Team CreateTestTeam()
    {
        return new Team("Test Team", Guid.NewGuid());
    }
}