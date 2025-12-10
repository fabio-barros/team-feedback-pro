using MediatR;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Feedbacks.Queries.GetPendingFeedbacks;

public record GetPendingFeedbacksQuery(
    Guid ManagerId,
    Guid TeamId,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedResult<FeedbackPendingResult>>>;