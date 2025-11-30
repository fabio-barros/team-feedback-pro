namespace TeamFeedbackPro.Application.Common.Models;

public record FeedbackPendingResult(
    Guid Id,
    Guid AuthorId,
    string AuthorName,
    Guid RecipientId,
    string RecipientName,
    string Type,
    string Category,
    string Content,
    bool IsAnonymous,
    string Status,
    DateTime CreatedAt
);