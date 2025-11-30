using System.ComponentModel.DataAnnotations;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Api.Contracts;

/// <summary>
/// Request model for creating a new feedback
/// </summary>
public record ReviewFeedbackRequest
{
    /// <summary>
    /// Feedback review (min 20, max 2000 characters)
    /// </summary>
    /// <example>Great job on the recent code review. Your attention to detail helped us catch several potential bugs.</example>
    [Required]
    [MinLength(20)]
    [MaxLength(2000)]
    public string Content { get; init; }
}