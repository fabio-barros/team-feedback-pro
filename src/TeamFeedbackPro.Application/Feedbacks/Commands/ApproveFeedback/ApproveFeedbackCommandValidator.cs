using FluentValidation;

namespace TeamFeedbackPro.Application.Feedbacks.Commands.ApproveFeedback;

public class ApproveFeedbackCommandValidator : AbstractValidator<ApproveFeedbackCommand>
{
    public ApproveFeedbackCommandValidator()
    {
        RuleFor(x => x.FeedbackId)
            .NotEmpty().WithMessage("Feedback Id is required");

        RuleFor(x => x.ManagerId)
            .NotEmpty().WithMessage("Manager Id is required");
    }
}