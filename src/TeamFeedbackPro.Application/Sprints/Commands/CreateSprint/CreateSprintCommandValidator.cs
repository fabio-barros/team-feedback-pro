using FluentValidation;

namespace TeamFeedbackPro.Application.Sprints.Commands.CreateSprint;

public class CreateSprintCommandValidator : AbstractValidator<CreateSprintCommand>
{
    public CreateSprintCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .WithMessage("Sprint name is required");

        RuleFor(x => x.StartAt)
            .NotNull()
            .WithMessage("Start date is required");
 
        RuleFor(x => x.EndAt)
            .NotNull()
            .WithMessage("Start date is required");

        RuleFor(x => x.Description)
            .MinimumLength(20).WithMessage("Content must be at least 20 characters")
            .MaximumLength(2000).WithMessage("Content must not exceed 2000 characters");
    }
}