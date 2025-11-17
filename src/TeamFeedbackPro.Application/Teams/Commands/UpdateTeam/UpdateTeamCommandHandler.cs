using MediatR;
using Microsoft.Extensions.Logging;
using System.IO.Pipelines;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Teams.Commands.UpdateTeam;

public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, Result<TeamResult>>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTeamCommandHandler> _logger;

    public UpdateTeamCommandHandler(
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTeamCommandHandler> logger)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TeamResult>> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating team {TeamId}", request.Id);

        var team = await _teamRepository.GetByIdAsync(request.Id, cancellationToken);
        if (team is null)
        {
            _logger.LogWarning("Team not found {TeamId}", request.Id);
            return Result.Failure<TeamResult>(ErrorMessages.TeamNotFound);
        }

        // Validate manager exists if provided
        if (request.ManagerId.HasValue)
        {
            var manager = await _userRepository.GetByIdAsync(request.ManagerId.Value, cancellationToken);
            if (manager is null)
            {
                _logger.LogWarning("Manager not found with Id {ManagerId}", request.ManagerId);
                return Result.Failure<TeamResult>(ErrorMessages.UserNotFound);
            }
        }

        team.UpdateName(request.Name);

        if (request.ManagerId.HasValue)
        {
            team.AssignManager(request.ManagerId.Value);
        }

        await _teamRepository.UpdateAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Team updated successfully {TeamId}", team.Id);

        return Result.Success(new TeamResult(
            team.Id,
            team.Name,
            team.ManagerId,
            team.CreatedAt,
            team.UpdatedAt
        ));
    }
}