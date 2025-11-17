using MediatR;
using Microsoft.Extensions.Logging;
using System.IO.Pipelines;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Teams.Queries.GetAllTeams;

public class GetAllTeamsQueryHandler(
    ITeamRepository teamRepository,
    ILogger<GetAllTeamsQueryHandler> logger)
    : IRequestHandler<GetAllTeamsQuery, Result<IEnumerable<TeamResult>>>
{
    public async Task<Result<IEnumerable<TeamResult>>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all teams");

        var teams = await teamRepository.GetAllAsync(cancellationToken);

        var teamResults = teams.Select(t => new TeamResult(
            t.Id,
            t.Name,
            t.ManagerId,
            t.CreatedAt,
            t.UpdatedAt
        ));

        logger.LogInformation("Retrieved {Count} teams", teamResults.Count());

        return Result.Success(teamResults);
    }
}