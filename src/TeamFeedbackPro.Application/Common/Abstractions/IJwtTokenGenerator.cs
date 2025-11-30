using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Common.Abstractions;
public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, UserRole role, Guid teamId);
}