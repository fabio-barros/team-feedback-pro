using FluentValidation;

namespace TeamFeedbackPro.Application.Feedbacks.Commands.CreateFeedback;

public class CreateFeedbackCommandValidator : AbstractValidator<CreateFeedbackCommand>
{
    public CreateFeedbackCommandValidator()
    {
        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author ID is required");

        RuleFor(x => x.RecipientId)
            .NotEmpty().WithMessage("Recipient ID is required")
            .NotEqual(x => x.AuthorId).WithMessage("Cannot send feedback to yourself");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid feedback type");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid feedback category");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MinimumLength(20).WithMessage("Content must be at least 20 characters")
            .MaximumLength(2000).WithMessage("Content must not exceed 2000 characters");

        RuleFor(x => x.ImprovementSuggestion)
            .MinimumLength(20).WithMessage("Improvement suggestion must be at least 20 characters")
            .MaximumLength(2000).WithMessage("Improvement suggestion must not exceed 2000 characters");
    }
}