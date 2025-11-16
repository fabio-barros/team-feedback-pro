using System.ComponentModel.DataAnnotations;

namespace TeamFeedBackPro.Infrastructure.Settings;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    [Required]
    [MinLength(32, ErrorMessage = "Secret must be at least 32 characters long.")]
    public string Secret { get; init; } = null!;

    [Required]
    public string Issuer { get; init; } = null!;

    [Required]
    public string Audience { get; init; } = null!;

    [Range(1, 365, ErrorMessage = "ExpiryDays must be between 1 and 365.")]
    public int ExpiryDays { get; init; } = 7;
}