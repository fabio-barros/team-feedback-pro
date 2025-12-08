using System.ComponentModel.DataAnnotations;

namespace TeamFeedbackPro.Api.Contracts;

/// <summary>
/// Request model for creating a new feedback
/// </summary>
public record CreateSprintRequest
{
    /// <summary>
    /// Sprint start date
    /// </summary>
    /// <example>30/11/2025</example>
    [Required]
    public required DateTime StartAt { get; init; }
    
    /// <summary>
    /// Sprint end date
    /// </summary>
    /// <example>07/12/2025</example>
    [Required]
    public required DateTime EndAt { get; init; }
    
    /// <summary>
    /// Sprint name
    /// </summary>
    /// <example>Sprint 1</example>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    /// Sprint description (min 20, max 2000 characters)
    /// </summary>
    /// <example>Just a example for a sprint description.</example>
    [MinLength(20)]
    [MaxLength(2000)]
    public required string Description { get; init; }

}