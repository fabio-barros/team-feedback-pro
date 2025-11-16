using Microsoft.Extensions.Options;

namespace TeamFeedBackPro.Infrastructure.Authentication;

public class JwtSettingsValidator : IValidateOptions<JwtSettings>
{
    public ValidateOptionsResult Validate(string? name, JwtSettings? options)
    {
        if (options is null)
            return ValidateOptionsResult.Fail("JwtSettings is null.");

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Secret) || options.Secret.Length < 32)
            errors.Add("JwtSettings:Secret must be provided and at least 32 characters long.");

        if (string.IsNullOrWhiteSpace(options.Issuer))
            errors.Add("JwtSettings:Issuer is required.");

        if (string.IsNullOrWhiteSpace(options.Audience))
            errors.Add("JwtSettings:Audience is required.");

        if (options.ExpiryDays <= 0)
            errors.Add("JwtSettings:ExpiryDays must be greater than zero.");

        return errors.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(errors);
    }
}