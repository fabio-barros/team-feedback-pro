using MediatR;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Feedbacks.Commands.CreateFeedback;

public record CreateFeedbackCommand(
    Guid AuthorId,
    Guid RecipientId,
    FeedbackType Type,
    FeedbackCategory Category,
    string Content,
    bool IsAnonymous, 
    Guid? FeelingId,
    string? ImprovementSuggestion
) : IRequest<Result<FeedbackResult>>;