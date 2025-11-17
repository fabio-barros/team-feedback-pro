using FluentValidation;

namespace TeamFeedbackPro.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name cannot be only whitespace");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role");
    }
}