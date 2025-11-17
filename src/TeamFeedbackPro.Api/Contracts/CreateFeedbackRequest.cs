using System.ComponentModel.DataAnnotations;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Api.Contracts;

/// <summary>
/// Request model for creating a new feedback
/// </summary>
public record CreateFeedbackRequest
{
    /// <summary>
    /// Recipient user ID (must be in the same team)
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Required]
    public required Guid RecipientId { get; init; }

    /// <summary>
    /// Feedback type: Positive, Constructive, or Critical
    /// </summary>
    /// <example>Positive</example>
    [Required]
    public required FeedbackType Type { get; init; }

    /// <summary>
    /// Feedback category
    /// </summary>
    /// <example>CodeQuality</example>
    [Required]
    public required FeedbackCategory Category { get; init; }

    /// <summary>
    /// Feedback content (min 20, max 2000 characters)
    /// </summary>
    /// <example>Great job on the recent code review. Your attention to detail helped us catch several potential bugs.</example>
    [Required]
    [MinLength(20)]
    [MaxLength(2000)]
    public required string Content { get; init; }

    /// <summary>
    /// Whether the feedback should be anonymous
    /// </summary>
    /// <example>false</example>
    public bool IsAnonymous { get; init; }
}