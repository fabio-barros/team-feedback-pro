using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Auth.Queries.GetMe;


public class GetMeQueryHandler(
    IUserRepository userRepository, 
    ILogger<GetMeQueryHandler> logger
    ) : IRequestHandler<GetMeQuery, Result<GetMeResult>>
{
    public async Task<Result<GetMeResult>> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("GetMe requested for UserId {UserId}", request.UserId);

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("GetMe: user not found {UserId}", request.UserId);
            return Result<GetMeResult>.Failure(ErrorMessages.UserNotFound);
        }

        logger.LogInformation("GetMe success for UserId {UserId}", user.Id);

        return Result.Success(new GetMeResult(
            user.Id, user.Email, user.Name, user.Role, user.TeamId, user.CreatedAt));
    }
}