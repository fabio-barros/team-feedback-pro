using System.ComponentModel.DataAnnotations;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Api.Contracts;

/// <summary>
/// Request model for updating a user
/// </summary>
public record UpdateUserRequest
{
    /// <summary>
    /// User full name
    /// </summary>
    /// <example>John Doe</example>
    [Required]
    [MaxLength(200)]
    public required string Name { get; init; }

    /// <summary>
    /// User role: Member, Manager, or Admin
    /// </summary>
    /// <example>Manager</example>
    [Required]
    public required UserRole Role { get; init; }

    /// <summary>
    /// Optional team ID to assign the user to
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid? TeamId { get; init; }
}