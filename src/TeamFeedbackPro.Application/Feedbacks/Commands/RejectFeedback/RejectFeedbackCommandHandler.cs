using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Feedbacks.Commands.RejectFeedback;

public class RejectFeedbackCommandHandler : IRequestHandler<RejectFeedbackCommand, Result<bool>>
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectFeedbackCommandHandler> _logger;

    public RejectFeedbackCommandHandler(
        IFeedbackRepository feedbackRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<RejectFeedbackCommandHandler> logger)
    {
        _feedbackRepository = feedbackRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(RejectFeedbackCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting feedback {FeedbackId}", request.FeedbackId);

        // Validating feedback exists
        var feedback = await _feedbackRepository.GetByIdAsync(request.FeedbackId);
        if (feedback is null)
        {
            _logger.LogWarning("Feedback not found {FeedbackId}", request.FeedbackId);
            return Result.Failure<bool>("Feedback not found");
        }
        
        if (feedback.Status != FeedbackStatus.Pending)
        {
            _logger.LogWarning("Feedback {FeedbackId} status is not pending", request.FeedbackId);
            return Result.Failure<bool>("Feedback is not pending");
        }
        
        // Validating manager exists
        var manager = await _userRepository.GetByIdAsync(request.ManagerId);
        if (manager is null)
        {
            _logger.LogWarning("Manager not found {ManagerId}", request.ManagerId);
            return Result.Failure<bool>(ErrorMessages.UserNotFound);
        }

        // Validate author exists
        var author = await _userRepository.GetByIdAsync(feedback.AuthorId, cancellationToken);
        if (author is null)
        {
            _logger.LogWarning("Author not found");
            return Result.Failure<bool>(ErrorMessages.UserNotFound);
        }

        // Validate recipient exists
        var recipient = await _userRepository.GetByIdAsync(feedback.RecipientId, cancellationToken);
        if (recipient is null)
        {
            _logger.LogWarning("Recipient not found");
            return Result.Failure<bool>(ErrorMessages.UserNotFound);
        }

        // Validate both are in the same team
        if (!author.TeamId.HasValue || author.TeamId != recipient.TeamId)
        {
            _logger.LogWarning("Author and recipient are not in the same team");
            return Result.Failure<bool>("Author and recipient must be in the same team");
        }

        feedback.Reject(manager.Id, request.Review);

        await _feedbackRepository.UpdateAsync(feedback, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Feedback rejected successfully {FeedbackId}", feedback.Id);

        return Result.Success(true);
    }
}