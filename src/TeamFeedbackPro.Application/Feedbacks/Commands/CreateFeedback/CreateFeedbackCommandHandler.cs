using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Application.Feedbacks.Commands.CreateFeedback;

public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, Result<FeedbackResult>>
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFeelingRepository _feelingRepository; 
    private readonly ISprintRepository _sprintRepository; 
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateFeedbackCommandHandler> _logger;

    public CreateFeedbackCommandHandler(
        IFeedbackRepository feedbackRepository,
        IUserRepository userRepository,
        IFeelingRepository feelingRepository,
        ISprintRepository sprintRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateFeedbackCommandHandler> logger)
    {
        _feedbackRepository = feedbackRepository;
        _userRepository = userRepository;
        _feelingRepository = feelingRepository;
        _sprintRepository = sprintRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<FeedbackResult>> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating feedback from {AuthorId} to {RecipientId}", request.AuthorId, request.RecipientId);

        // Validate author exists
        var author = await _userRepository.GetByIdAsync(request.AuthorId, cancellationToken);
        if (author is null)
        {
            _logger.LogWarning("Author not found {AuthorId}", request.AuthorId);
            return Result.Failure<FeedbackResult>(ErrorMessages.UserNotFound);
        }

        // Validate recipient exists
        var recipient = await _userRepository.GetByIdAsync(request.RecipientId, cancellationToken);
        if (recipient is null)
        {
            _logger.LogWarning("Recipient not found {RecipientId}", request.RecipientId);
            return Result.Failure<FeedbackResult>("Recipient user not found");
        }

        // Validate both are in the same team
        if (!author.TeamId.HasValue || author.TeamId != recipient.TeamId)
        {
            _logger.LogWarning("Author {AuthorId} and recipient {RecipientId} are not in the same team",
                request.AuthorId, request.RecipientId);
            return Result.Failure<FeedbackResult>("Author and recipient must be in the same team");
        }

        // Validate if selected feeling, if selected, exists
        Feeling? feeling = null;
        if (request.FeelingId.HasValue)
        {
            feeling = await _feelingRepository.GetByIdAsync(request.FeelingId.Value, cancellationToken);
            if (feeling == null)
            {
                _logger.LogWarning("Feeling could not be found by Feeling id {FeelingId} ",
                    request.FeelingId);
                return Result.Failure<FeedbackResult>("Feeling could not be found by Feeling id ");
            }
        }

        // Validate if there is a actual sprint going
        var sprint = await _sprintRepository.GetActualSprintAsync(author.TeamId.Value);
        if (sprint == null)
        {
            _logger.LogWarning("There is no sprint going");
            return Result.Failure<FeedbackResult>("There is no sprint going");
        }

        var feedback = new Feedback(
            request.AuthorId,
            request.RecipientId,
            request.Type,
            request.Category,
            request.Content,
            request.IsAnonymous,
            author.TeamId.Value,
            sprint.Id,
            request.FeelingId
        );

        await _feedbackRepository.AddAsync(feedback, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Feedback created successfully {FeedbackId}", feedback.Id);

        return Result.Success(new FeedbackResult(
            feedback.Id,
            feedback.AuthorId,
            feedback.RecipientId,
            feedback.Type.ToString(),
            feedback.Category.ToString(),
            feedback.Content,
            feedback.IsAnonymous,
            feedback.Status.ToString(),
            feeling?.Name,
            feedback.Sprint?.Name,
            feedback.CreatedAt
        ));
    }
}