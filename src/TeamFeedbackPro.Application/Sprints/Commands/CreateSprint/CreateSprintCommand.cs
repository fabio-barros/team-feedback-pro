using MediatR;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Sprints.Commands.CreateSprint;

public record CreateSprintCommand(
    Guid ManagerId,
    string Name,
    string? Description,
    DateTime StartAt,
    DateTime EndAt
) : IRequest<Result<SprintResult>>;