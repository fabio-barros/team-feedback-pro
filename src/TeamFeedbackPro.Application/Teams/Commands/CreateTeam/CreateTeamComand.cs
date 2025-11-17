using MediatR;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Teams.Commands.CreateTeam;

public record CreateTeamCommand(
    string Name,
    Guid? ManagerId
) : IRequest<Result<TeamResult>>;