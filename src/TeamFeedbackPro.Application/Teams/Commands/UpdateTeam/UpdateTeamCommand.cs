using MediatR;
using System.IO.Pipelines;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Teams.Commands.UpdateTeam;

public record UpdateTeamCommand(
    Guid Id,
    string Name,
    Guid? ManagerId
) : IRequest<Result<TeamResult>>;