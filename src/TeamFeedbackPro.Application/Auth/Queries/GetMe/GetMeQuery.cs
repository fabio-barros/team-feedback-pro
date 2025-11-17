using MediatR;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Auth.Queries.GetMe;

public record GetMeQuery(Guid UserId) : IRequest<Result<GetMeResult>>;

public sealed record GetMeResult(
    Guid UserId,
    string Email,
    string Name,
    UserRole Role,
    Guid? TeamId,
    DateTimeOffset CreatedAt
);

public static class UserExtensions
{
    public static GetMeResult ToResult(this User user)
    {
        return new GetMeResult(
            user.Id,
            user.Email,
            user.Name,
            user.Role,
            user.TeamId,
            user.CreatedAt
        );
    }
}
