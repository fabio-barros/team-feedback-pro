using MediatR;
using System.IO.Pipelines;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Teams.Queries.GetTeam;

public record GetTeamQuery(Guid Id) : IRequest<Result<TeamResult>>;