namespace TeamFeedbackPro.Application.Common.Models;

public record UserResult(
    Guid Id,
    string Email,
    string Name,
    string Role,
    Guid? TeamId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);