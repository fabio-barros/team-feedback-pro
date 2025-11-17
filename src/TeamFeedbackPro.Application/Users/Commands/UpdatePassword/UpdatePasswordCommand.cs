using MediatR;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Users.Commands.UpdatePassword;

public record UpdatePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<Result<bool>>;