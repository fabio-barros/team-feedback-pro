using MediatR;

namespace TeamFeedbackPro.Application.Common.Models.GetTeam;

public record GetTeamQuery(Guid Id) : IRequest<Result<TeamResult>>;