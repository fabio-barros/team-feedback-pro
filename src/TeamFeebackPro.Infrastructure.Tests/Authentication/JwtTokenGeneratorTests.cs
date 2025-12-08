using FluentAssertions;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using TeamFeedBackPro.Infrastructure.Authentication;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Infrastructure.Tests.Authentication;

public class JwtTokenGeneratorTests
{
    private static JwtSettings CreateValidSettings() => new JwtSettings
    {
        Secret = "12345678901234567890123456789012", // 32 chars
        Issuer = "issuer",
        Audience = "audience",
        ExpiryDays = 7
    };

    private static JwtTokenGenerator CreateGenerator(JwtSettings? settings = null)
    {
        var options = Options.Create(settings ?? CreateValidSettings());
        return new JwtTokenGenerator(options);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtWithExpectedClaims()
    {
        // Arrange
        var generator = CreateGenerator();
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var role = UserRole.Member;
        var teamId = Guid.NewGuid();

        // Act
        var token = generator.GenerateToken(userId, email, role, teamId);

        // Assert
        token.Should().NotBeNullOrWhiteSpace();

        // Decode and validate claims
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value.Should().Be(userId.ToString());
        jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value.Should().Be(email);
        jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.Should().Be(role.ToString());
        jwt.Claims.FirstOrDefault(c => c.Type == "teamId")?.Value.Should().Be(teamId.ToString());
        jwt.Issuer.Should().Be("issuer");
        jwt.Audiences.Should().Contain("audience");
        jwt.ValidTo.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void GenerateToken_WithDifferentRoles_ShouldSetRoleClaim()
    {
        // Arrange
        var generator = CreateGenerator();
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var teamId = Guid.NewGuid();

        foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
        {
            // Act
            var token = generator.GenerateToken(userId, email, role, teamId);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.Should().Be(role.ToString());
        }
    }
}