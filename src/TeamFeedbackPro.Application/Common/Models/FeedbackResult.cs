namespace TeamFeedbackPro.Application.Common.Models;

public record FeedbackResult(
    Guid Id,
    Guid AuthorId,
    Guid RecipientId,
    string Type,
    string Category,
    string Content,
    bool IsAnonymous,
    string Status,
    DateTime CreatedAt
);