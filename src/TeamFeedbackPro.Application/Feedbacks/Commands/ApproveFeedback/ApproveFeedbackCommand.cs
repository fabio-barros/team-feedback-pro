using MediatR;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Feedbacks.Commands.CreateFeedback;

public record ApproveFeedbackCommand(
    Guid FeedbackId,
    Guid ManagerId
) : IRequest<Result<bool>>;