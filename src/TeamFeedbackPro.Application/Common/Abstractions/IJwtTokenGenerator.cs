using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Common.Interfaces;
public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, UserRole role);
}