using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedBackPro.Infrastructure.Persistence;

namespace TeamFeedBackPro.Infrastructure.Persistence.Repositories;

public class FeelingRepository(ApplicationDbContext context) : IFeelingRepository
{
    public async Task<IEnumerable<Feeling>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Feelings
            .ToListAsync(cancellationToken);
    }

    public async Task<Feeling?> GetByIdAsync(Guid feelingId, CancellationToken cancellationToken = default)
    {
        return await context.Feelings
            .FirstOrDefaultAsync(e => e.Id == feelingId);
    }

}
