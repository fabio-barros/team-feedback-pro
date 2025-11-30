using MediatR;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Feedbacks.Commands.ApproveFeedback;

public record ApproveFeedbackCommand(
    Guid FeedbackId,
    Guid ManagerId,
    string? Review
) : IRequest<Result<bool>>;