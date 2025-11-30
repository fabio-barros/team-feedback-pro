using MediatR;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Feedbacks.Commands.RejectFeedback;

public record RejectFeedbackCommand(
    Guid FeedbackId,
    Guid ManagerId,
    string Review
) : IRequest<Result<bool>>;