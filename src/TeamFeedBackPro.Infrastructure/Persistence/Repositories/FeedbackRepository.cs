using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedBackPro.Infrastructure.Persistence.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly ApplicationDbContext _context;

    public FeedbackRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Feedbacks
            .Include(f => f.Author)
            .Include(f => f.Recipient)
            .Include(f => f.Reviewer)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<Feedback> AddAsync(Feedback feedback, CancellationToken cancellationToken = default)
    {
        await _context.Feedbacks.AddAsync(feedback, cancellationToken);
        return feedback;
    }

    public Task UpdateAsync(Feedback feedback, CancellationToken cancellationToken = default)
    {
        _context.Feedbacks.Update(feedback);
        return Task.CompletedTask;
    }

    public async Task<(IEnumerable<Feedback> Items, int TotalCount)> GetSentByAuthorAsync(
        Guid authorId,
        FeedbackStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Feedbacks
            .Include(f => f.Recipient)
            .Include(f => f.Reviewer)
            .Where(f => f.AuthorId == authorId);

        if (status.HasValue)
        {
            query = query.Where(f => f.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<Feedback>> GetPendingByRecipientAsync(
        Guid recipientId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Feedbacks
            .Include(f => f.Author)
            .Where(f => f.RecipientId == recipientId && f.Status == FeedbackStatus.Pending)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}