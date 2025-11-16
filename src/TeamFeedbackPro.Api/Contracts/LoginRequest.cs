using System.ComponentModel.DataAnnotations;

namespace TeamFeedbackPro.Api.Contracts;

/// <summary>
/// Request model for user login
/// </summary>
public record LoginRequest
{
    /// <summary>
    /// User email address (case-insensitive)
    /// </summary>
    /// <example>user@example.com</example>
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    /// User password
    /// </summary>
    /// <example>SecurePass123</example>
    [Required]
    public required string Password { get; init; }
}