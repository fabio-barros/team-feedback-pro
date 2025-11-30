using FluentValidation;

namespace TeamFeedbackPro.Application.Feedbacks.Commands.RejectFeedback;

public class RejectFeedbackCommandValidator : AbstractValidator<RejectFeedbackCommand>
{
    public RejectFeedbackCommandValidator()
    {
        RuleFor(x => x.FeedbackId)
            .NotEmpty().WithMessage("Feedback Id is required");

        RuleFor(x => x.ManagerId)
            .NotEmpty().WithMessage("Manager Id is required");

        RuleFor(x => x.Review)
            .NotEmpty().WithMessage("Review is required")
            .MinimumLength(20).WithMessage("Review must be at least 20 characters")
            .MaximumLength(2000).WithMessage("Review must not exceed 2000 characters");
    }
}