using MediatR;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Users.Queries.GetUser;

public record GetUserQuery(Guid Id) : IRequest<Result<UserResult>>;