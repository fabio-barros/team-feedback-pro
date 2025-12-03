using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Feedbacks.Queries.GetSentFeedbacks;

public class GetSentFeedbacksQueryHandler(
    IFeedbackRepository feedbackRepository,
    ILogger<GetSentFeedbacksQueryHandler> logger)
    : IRequestHandler<GetSentFeedbacksQuery, Result<PaginatedResult<FeedbackResult>>>
{
    public async Task<Result<PaginatedResult<FeedbackResult>>> Handle(GetSentFeedbacksQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting sent feedbacks for author {AuthorId}, page {Page}", request.AuthorId, request.Page);

        var (items, totalCount) = await feedbackRepository.GetSentByAuthorAsync(
            request.AuthorId,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        var feedbackResults = items.Select(f => new FeedbackResult(
            f.Id,
            f.AuthorId,
            f.RecipientId,
            f.Type.ToDescription(),
            f.Category.ToDescription(),
            f.Content,
            f.IsAnonymous,
            f.Status.ToDescription(),
            f.Feeling?.Name,
            f.CreatedAt
        ));

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var paginatedResult = new PaginatedResult<FeedbackResult>(
            feedbackResults,
            request.Page,
            request.PageSize,
            totalCount,
            totalPages
        );

        logger.LogInformation("Retrieved {Count} feedbacks", feedbackResults.Count());

        return Result.Success(paginatedResult);
    }
}