namespace TeamFeedbackPro.Application.Common.Models;

public record TeamResult(
    Guid Id,
    string Name,
    Guid? ManagerId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);