using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Teams.Commands.DeleteTeam;

public class DeleteTeamCommandHandler(
    ITeamRepository teamRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteTeamCommandHandler> logger)
    : IRequestHandler<DeleteTeamCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting team {TeamId}", request.Id);

        var team = await teamRepository.GetByIdAsync(request.Id, cancellationToken);
        if (team is null)
        {
            logger.LogWarning("Team not found {TeamId}", request.Id);
            return Result.Failure<bool>(ErrorMessages.TeamNotFound);
        }

        await teamRepository.DeleteAsync(team, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Team deleted successfully {TeamId}", request.Id);

        return Result.Success(true);
    }
}