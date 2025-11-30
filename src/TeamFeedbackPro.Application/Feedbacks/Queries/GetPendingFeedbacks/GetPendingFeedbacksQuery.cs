using MediatR;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Feedbacks.Queries.GetReceivedFeedbacks;

public record GetPendingFeedbacksQuery(
    Guid ManagerId,
    Guid TeamId,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedResult<FeedbackPendingResult>>>;