using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Application.Common.Abstractions;

public interface ISprintRepository
{
    Task<Sprint?> GetActualSprintAsync (Guid teamId);
    Task<Sprint> AddAsync(Sprint sprint, CancellationToken cancellationToken = default);
    // Task<bool> ExistAsync(
    //     DateTime startAt,
    //     DateTime endAt,
    //     Guid teamId,
    //     CancellationToken cancellationToken = default);
}