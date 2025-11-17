using System.ComponentModel.DataAnnotations;

namespace TeamFeedbackPro.Api.Contracts;

/// <summary>
/// Request model for updating user password
/// </summary>
public record UpdatePasswordRequest
{
    /// <summary>
    /// Current password
    /// </summary>
    /// <example>OldPass123</example>
    [Required]
    public required string CurrentPassword { get; init; }

    /// <summary>
    /// New password (min 8 chars, 1 uppercase, 1 lowercase, 1 number)
    /// </summary>
    /// <example>NewPass123</example>
    [Required]
    [MinLength(8)]
    public required string NewPassword { get; init; }
}