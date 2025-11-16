using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Common;

public record UserDto(
    Guid Id,
    string Email,
    string Name,
    string Role,
    Guid? TeamId
);

public sealed record GetMeResult(
    Guid UserId,
    string Email,
    string Name,
    UserRole Role,
    Guid? TeamId,
    DateTimeOffset CreatedAt
);