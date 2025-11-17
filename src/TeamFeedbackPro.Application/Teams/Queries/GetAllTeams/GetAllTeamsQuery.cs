using MediatR;
using System.IO.Pipelines;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Teams.Queries.GetAllTeams;

public record GetAllTeamsQuery : IRequest<Result<IEnumerable<TeamResult>>>;