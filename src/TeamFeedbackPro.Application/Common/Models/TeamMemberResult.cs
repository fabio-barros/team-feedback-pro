namespace TeamFeedbackPro.Application.Common.Models;

public record TeamMemberResult(
    Guid Id,
    string Name,
    string Email,
    string Role
);