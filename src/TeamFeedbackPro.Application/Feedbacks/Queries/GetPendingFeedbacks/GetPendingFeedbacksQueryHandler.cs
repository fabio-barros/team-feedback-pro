using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Application.Feedbacks.Queries.GetReceivedFeedbacks;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Feedbacks.Queries.GetSentFeedbacks;

public class GetPendingFeedbacksQueryHandler(
    IFeedbackRepository feedbackRepository,
    ILogger<GetPendingFeedbacksQueryHandler> logger)
    : IRequestHandler<GetPendingFeedbacksQuery, Result<PaginatedResult<FeedbackPendingResult>>>
{
    public async Task<Result<PaginatedResult<FeedbackPendingResult>>> Handle(GetPendingFeedbacksQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting pending feedbacks for manager {managerId}, page {Page}", request.ManagerId, request.Page);

        var (items, totalCount) = await feedbackRepository.GetPendingByManagerAsync(
            request.TeamId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var feedbackPendingResults = items.Select(f => new FeedbackPendingResult(
            f.Id,
            f.AuthorId,
            f.Author.Name,
            f.RecipientId,
            f.Recipient.Name,
            f.Type.ToDescription(),
            f.Category.ToDescription(),
            f.Content,
            f.IsAnonymous,
            f.Status.ToDescription(),
            f.Feeling?.Name,
            f.CreatedAt
        ));

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var paginatedResult = new PaginatedResult<FeedbackPendingResult>(
            feedbackPendingResults,
            request.Page,
            request.PageSize,
            totalCount,
            totalPages
        );

        logger.LogInformation("Retrieved {Count} feedbacks", feedbackPendingResults.Count());

        return Result.Success(paginatedResult);
    }
}