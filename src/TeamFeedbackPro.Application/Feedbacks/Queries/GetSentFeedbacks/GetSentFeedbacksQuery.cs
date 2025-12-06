using MediatR;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Feedbacks.Queries.GetSentFeedbacks;

public record GetSentFeedbacksQuery(
    Guid AuthorId,
    FeedbackStatus? Status,
    int Page = 1,
    int PageSize = 11
) : IRequest<Result<PaginatedResult<FeedbackResult>>>;