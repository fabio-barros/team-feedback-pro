using MediatR;
using Microsoft.Extensions.Logging;
using System.IO.Pipelines;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Application.Teams.Commands.CreateTeam;

public class CreateTeamCommandHandler(
    ITeamRepository teamRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateTeamCommandHandler> logger)
    : IRequestHandler<CreateTeamCommand, Result<TeamResult>>
{
    public async Task<Result<TeamResult>> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating team with name {TeamName}", request.Name);

        // Validate manager exists if provided
        if (request.ManagerId.HasValue)
        {
            var manager = await userRepository.GetByIdAsync(request.ManagerId.Value, cancellationToken);
            if (manager is null)
            {
                logger.LogWarning("Manager not found with Id {ManagerId}", request.ManagerId);
                return Result.Failure<TeamResult>(ErrorMessages.UserNotFound);
            }
        }

        var team = new Team(request.Name, request.ManagerId);

        await teamRepository.AddAsync(team, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Team created successfully with Id {TeamId}", team.Id);

        return Result.Success(new TeamResult(
            team.Id,
            team.Name,
            team.ManagerId,
            team.CreatedAt,
            team.UpdatedAt
        ));
    }
}