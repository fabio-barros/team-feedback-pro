using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Users.Queries.GetTeamMembers;

public class GetTeamMembersQueryHandler(
    IUserRepository userRepository,
    ILogger<GetTeamMembersQueryHandler> logger)
    : IRequestHandler<GetTeamMembersQuery, Result<IEnumerable<TeamMemberResult>>>
{
    public async Task<Result<IEnumerable<TeamMemberResult>>> Handle(GetTeamMembersQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting team members for user {UserId}", request.UserId);

        var currentUser = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (currentUser is null)
        {
            logger.LogWarning("User not found {UserId}", request.UserId);
            return Result.Failure<IEnumerable<TeamMemberResult>>(ErrorMessages.UserNotFound);
        }

        if (!currentUser.TeamId.HasValue)
        {
            logger.LogInformation("User {UserId} has no team", request.UserId);
            return Result.Success(Enumerable.Empty<TeamMemberResult>());
        }

        var teamMembers = await userRepository.GetByTeamIdAsync(currentUser.TeamId.Value, cancellationToken);

        var results = teamMembers
            .Where(u => u.Id != request.UserId) // Exclude current user
            .Select(u => new TeamMemberResult(
                u.Id,
                u.Name,
                u.Email,
                u.Role.ToString()
            ));

        logger.LogInformation("Retrieved {Count} team members", results.Count());

        return Result.Success(results);
    }
}