using System.ComponentModel.DataAnnotations;

namespace TeamFeedbackPro.Api.Contracts;

/// <summary>
/// Request model for creating a new team
/// </summary>
public record CreateTeamRequest
{
    /// <summary>
    /// Team name
    /// </summary>
    /// <example>Engineering Team</example>
    [Required]
    [MaxLength(200)]
    public required string Name { get; init; }

    /// <summary>
    /// Optional manager user ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid? ManagerId { get; init; }
}