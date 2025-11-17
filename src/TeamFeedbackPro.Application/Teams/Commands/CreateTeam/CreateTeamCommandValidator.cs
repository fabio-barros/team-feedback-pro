using FluentValidation;

namespace TeamFeedbackPro.Application.Teams.Commands.CreateTeam;

public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required")
            .MaximumLength(200).WithMessage("Team name must not exceed 200 characters")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Team name cannot be only whitespace");
    }
}