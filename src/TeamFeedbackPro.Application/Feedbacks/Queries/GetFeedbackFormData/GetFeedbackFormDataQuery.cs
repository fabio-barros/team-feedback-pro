using MediatR;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Application.Common.Models.FeedbackForm;

namespace TeamFeedbackPro.Application.Feedbacks.Queries.GetFeedbackFormData;

public record GetFeedbackFormDataQuery(
    Guid AuthorId
) : IRequest<Result<FeedbackFormDataResult>>;