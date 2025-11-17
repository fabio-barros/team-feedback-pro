using MediatR;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string Name,
    UserRole Role,
    Guid? TeamId = null
) : IRequest<Result<RegisterResult>>;

public record RegisterResult(
    Guid UserId,
    string Email,
    string Name,
    UserRole Role,
    Guid? TeamId,
    DateTimeOffset CreatedAt
);