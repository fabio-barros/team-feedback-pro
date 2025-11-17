using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Users.Queries.GetUser;

public class GetUserQueryHandler(
    IUserRepository userRepository,
    ILogger<GetUserQueryHandler> logger)
    : IRequestHandler<GetUserQuery, Result<UserResult>>
{
    public async Task<Result<UserResult>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user {UserId}", request.Id);

        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("User not found {UserId}", request.Id);
            return Result.Failure<UserResult>(ErrorMessages.UserNotFound);
        }

        logger.LogInformation("User retrieved successfully {UserId}", user.Id);

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