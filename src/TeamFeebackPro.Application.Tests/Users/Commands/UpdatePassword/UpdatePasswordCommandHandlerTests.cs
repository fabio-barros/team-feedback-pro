using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Users.Commands.UpdatePassword;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Tests.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdatePasswordCommandHandler>> _loggerMock;
    private readonly UpdatePasswordCommandHandler _handler;

    public UpdatePasswordCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdatePasswordCommandHandler>>();

        _handler = new UpdatePasswordCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidPasswordChange_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentPassword = "OldPassword123";
        var newPassword = "NewPassword456";
        var oldHash = "old_hashed_password";
        var newHash = "new_hashed_password";

        var user = new User("test@example.com", oldHash, "Test User", UserRole.Member, null);
        var command = new UpdatePasswordCommand(userId, currentPassword, newPassword);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(currentPassword, oldHash))
            .Returns(true);

        _passwordHasherMock
            .Setup(x => x.HashPassword(newPassword))
            .Returns(newHash);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        _passwordHasherMock.Verify(x => x.VerifyPassword(currentPassword, oldHash), Times.Once);
        _passwordHasherMock.Verify(x => x.HashPassword(newPassword), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingUser_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdatePasswordCommand(userId, "OldPassword123", "NewPassword456");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorMessages.UserNotFound);

        _passwordHasherMock.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithIncorrectCurrentPassword_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentPassword = "WrongPassword";
        var newPassword = "NewPassword456";
        var oldHash = "old_hashed_password";

        var user = new User("test@example.com", oldHash, "Test User", UserRole.Member, null);
        var command = new UpdatePasswordCommand(userId, currentPassword, newPassword);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(currentPassword, oldHash))
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Current password is incorrect");

        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CallsPasswordHasher()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentPassword = "OldPassword123";
        var newPassword = "NewPassword456";
        var oldHash = "old_hashed_password";
        var newHash = "new_hashed_password";

        var user = new User("test@example.com", oldHash, "Test User", UserRole.Member, null);
        var command = new UpdatePasswordCommand(userId, currentPassword, newPassword);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(currentPassword, oldHash))
            .Returns(true);

        _passwordHasherMock
            .Setup(x => x.HashPassword(newPassword))
            .Returns(newHash);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _passwordHasherMock.Verify(x => x.VerifyPassword(currentPassword, oldHash), Times.Once);
        _passwordHasherMock.Verify(x => x.HashPassword(newPassword), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdatesUserPassword()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentPassword = "OldPassword123";
        var newPassword = "NewPassword456";
        var oldHash = "old_hashed_password";
        var newHash = "new_hashed_password";

        var user = new User("test@example.com", oldHash, "Test User", UserRole.Member, null);
        var command = new UpdatePasswordCommand(userId, currentPassword, newPassword);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(currentPassword, oldHash))
            .Returns(true);

        _passwordHasherMock
            .Setup(x => x.HashPassword(newPassword))
            .Returns(newHash);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}