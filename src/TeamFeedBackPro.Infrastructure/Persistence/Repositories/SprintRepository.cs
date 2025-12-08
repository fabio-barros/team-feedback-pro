using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedBackPro.Infrastructure.Persistence.Repositories;

public class SprintRepository(ApplicationDbContext context) : ISprintRepository
{
    public async Task<Sprint?> GetActualSprintAsync(Guid teamId)
    {
        var now = DateTime.Now;
        return await context.Sprints
            .FirstOrDefaultAsync(s => s.TeamId == teamId && 
                s.StartAt.Date <= now &&
                now <= s.EndAt.Date);
    }

    public async Task<Sprint> AddAsync(Sprint sprint, CancellationToken cancellationToken = default)
    {
        await context.Sprints.AddAsync(sprint, cancellationToken);
        return sprint;
    }

    public async Task<bool> ExistAsync(
        DateTime startAt,
        DateTime endAt,
        Guid teamId,
        CancellationToken cancellationToken = default)
    {
        return context.Sprints.Any(s => s.TeamId == teamId &&
            ((startAt.Date <= s.StartAt.Date && s.StartAt.Date <= endAt.Date) ||
                (startAt.Date <= s.EndAt.Date && s.EndAt.Date <= endAt.Date)));
    }
}