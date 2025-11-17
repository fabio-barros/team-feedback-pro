using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(
    IUserRepository userRepository,
    ITeamRepository teamRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateUserCommandHandler> logger)
    : IRequestHandler<UpdateUserCommand, Result<UserResult>>
{
    public async Task<Result<UserResult>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating user {UserId}", request.Id);

        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found {UserId}", request.Id);
            return Result.Failure<UserResult>(ErrorMessages.UserNotFound);
        }

        // Validate team exists if provided
        if (request.TeamId.HasValue)
        {
            var team = await teamRepository.GetByIdAsync(request.TeamId.Value, cancellationToken);
            if (team is null)
            {
                logger.LogWarning("Team not found with Id {TeamId}", request.TeamId);
                return Result.Failure<UserResult>(ErrorMessages.TeamNotFound);
            }
        }

        // Update user properties
        user.UpdateRole(request.Role);

        if (request.TeamId.HasValue)
        {
            user.AssignToTeam(request.TeamId.Value);
        }

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User updated successfully {UserId}", user.Id);

        return Result.Success(new UserResult(
            user.Id,
            user.Email,
            user.Name,
            user.Role.ToString(),
            user.TeamId,
            user.CreatedAt,
            user.UpdatedAt
        ));
    }
}