using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Application.Common.Abstractions;

public interface ISprintRepository
{
    Task<Sprint?> GetActualSprintAsync (Guid teamId);
}