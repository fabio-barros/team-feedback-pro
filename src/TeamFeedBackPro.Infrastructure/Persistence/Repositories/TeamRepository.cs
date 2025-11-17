using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedBackPro.Infrastructure.Persistence.Repositories;

public class TeamRepository(ApplicationDbContext context) : ITeamRepository
{
    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Team>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Teams
            .Include(t => t.Members)
            .ToListAsync(cancellationToken);
    }

    public async Task<Team> AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        await context.Teams.AddAsync(team, cancellationToken);
        return team;
    }

    public Task UpdateAsync(Team team, CancellationToken cancellationToken = default)
    {
        context.Teams.Update(team);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Team team, CancellationToken cancellationToken = default)
    {
        context.Teams.Remove(team);
        return Task.CompletedTask;
    }
}