namespace TeamFeedbackPro.Application.Common;
public record AuthenticationResult(
    string Token,
    Guid UserId,
    string Email,
    string Name,
    string Role
);