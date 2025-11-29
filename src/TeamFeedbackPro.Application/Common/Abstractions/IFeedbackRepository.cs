using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Common.Abstractions;

public interface IFeedbackRepository
{
    Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Feedback> AddAsync(Feedback feedback, CancellationToken cancellationToken = default);
    Task UpdateAsync(Feedback feedback, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Feedback> Items, int TotalCount)> GetSentByAuthorAsync(
        Guid authorId,
        FeedbackStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<(IEnumerable<Feedback> Items, int TotalCount)> GetReceviedByUserAsync(
        Guid userId,
        FeedbackStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Feedback>> GetPendingByRecipientAsync(Guid recipientId, CancellationToken cancellationToken = default);
}