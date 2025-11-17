using MediatR;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Users.Queries.GetTeamMembers;

public record GetTeamMembersQuery(Guid UserId) : IRequest<Result<IEnumerable<TeamMemberResult>>>;