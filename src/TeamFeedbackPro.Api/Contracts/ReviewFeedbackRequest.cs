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
    /// <example>Excelente observação sobre o comportamento do seu companheiro.</example>
    [Required]
    [MinLength(20)]
    [MaxLength(2000)]
    public string Review { get; init; }
}