using MediatR;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Teams.Commands.DeleteTeam;

public record DeleteTeamCommand(Guid Id) : IRequest<Result<bool>>;