using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(
    IUserRepository userRepository,
    ILogger<GetAllUsersQueryHandler> logger)
    : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserResult>>>
{
    public async Task<Result<IEnumerable<UserResult>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all users");

        var users = await userRepository.GetAllAsync(cancellationToken);

        var userResults = users.Select(u => new UserResult(
            u.Id,
            u.Email,
            u.Name,
            u.Role.ToString(),
            u.TeamId,
            u.CreatedAt,
            u.UpdatedAt
        ));

        logger.LogInformation("Retrieved {Count} users", userResults.Count());

        return Result.Success(userResults);
    }
}