using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedBackPro.Infrastructure.Persistence;

namespace TeamFeedbackPro.Infrastructure.Persistence.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _context;

    public TeamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Team> AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        await _context.Teams.AddAsync(team, cancellationToken);
        return team;
    }

    public Task UpdateAsync(Team team, CancellationToken cancellationToken = default)
    {
        _context.Teams.Update(team);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Team>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Members)
            .ToListAsync(cancellationToken);
    }
}