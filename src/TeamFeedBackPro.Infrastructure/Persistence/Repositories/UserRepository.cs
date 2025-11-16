using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedBackPro.Infrastructure.Persistence.Repositories;


public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Include(u => u.Team)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Include(u => u.Team)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
        return user;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<User>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Where(u => u.TeamId == teamId)
            .ToListAsync(cancellationToken);
    }
}