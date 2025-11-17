using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    ILogger<UpdatePasswordCommandHandler> logger)
    : IRequestHandler<UpdatePasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating password for user {UserId}", request.UserId);

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found {UserId}", request.UserId);
            return Result.Failure<bool>(ErrorMessages.UserNotFound);
        }

        // Verify current password
        if (!passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            logger.LogWarning("Invalid current password for user {UserId}", request.UserId);
            return Result.Failure<bool>("Current password is incorrect");
        }

        // Hash and update new password
        var newPasswordHash = passwordHasher.HashPassword(request.NewPassword);
        user.UpdatePassword(newPasswordHash);

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Password updated successfully for user {UserId}", user.Id);

        return Result.Success(true);
    }
}