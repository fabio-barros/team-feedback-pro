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
}