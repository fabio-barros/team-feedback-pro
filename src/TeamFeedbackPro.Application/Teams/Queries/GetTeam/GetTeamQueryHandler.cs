using MediatR;
using Microsoft.Extensions.Logging;
using System.IO.Pipelines;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Teams.Queries.GetTeam;

public class GetTeamQueryHandler(
    ITeamRepository teamRepository,
    ILogger<GetTeamQueryHandler> logger)
    : IRequestHandler<GetTeamQuery, Result<TeamResult>>
{
    public async Task<Result<TeamResult>> Handle(GetTeamQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting team {TeamId}", request.Id);

        var team = await teamRepository.GetByIdAsync(request.Id, cancellationToken);

        if (team is null)
        {
            logger.LogWarning("Team not found {TeamId}", request.Id);
            return Result.Failure<TeamResult>(ErrorMessages.TeamNotFound);
        }

        logger.LogInformation("Team retrieved successfully {TeamId}", team.Id);

        return Result.Success(new TeamResult(
            team.Id,
            team.Name,
            team.ManagerId,
            team.CreatedAt,
            team.UpdatedAt
        ));
    }
}