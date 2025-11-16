using System.ComponentModel.DataAnnotations;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Api.Contracts;

/// <summary>
/// Request model for user registration
/// </summary>
public record RegisterRequest
{
    /// <summary>
    /// User email address (must be unique)
    /// </summary>
    /// <example>user@example.com</example>
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    /// User password (min 8 characters, 1 uppercase, 1 lowercase, 1 number)
    /// </summary>
    /// <example>SecurePass123</example>
    [Required]
    [MinLength(8)]
    public required string Password { get; init; }

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
    /// <example>Member</example>
    [Required]
    public required UserRole Role { get; init; }

    /// <summary>
    /// Optional team ID to assign the user to
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid? TeamId { get; init; }
}