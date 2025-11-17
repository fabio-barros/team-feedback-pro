using FluentValidation;

namespace TeamFeedbackPro.Application.Teams.Commands.UpdateTeam;

public class UpdateTeamCommandValidator : AbstractValidator<UpdateTeamCommand>
{
    public UpdateTeamCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Team ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required")
            .MaximumLength(200).WithMessage("Team name must not exceed 200 characters")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Team name cannot be only whitespace");
    }
}