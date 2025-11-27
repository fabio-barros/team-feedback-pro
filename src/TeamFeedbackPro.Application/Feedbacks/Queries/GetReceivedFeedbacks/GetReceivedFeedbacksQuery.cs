using MediatR;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Feedbacks.Queries.GetReceivedFeedbacks;

public record GetReceivedFeedbacksQuery(
    Guid UserId,
    FeedbackStatus? Status,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedResult<FeedbackResult>>>;