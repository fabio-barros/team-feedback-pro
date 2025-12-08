namespace TeamFeedbackPro.Application.Common.Models;

public record SprintResult(
    Guid Id,
    Guid TeamId,
    string Name,
    string Description,
    DateTime StartAt,
    DateTime EndAt,
    DateTime CreatedAt
);