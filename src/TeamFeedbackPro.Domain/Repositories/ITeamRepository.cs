using System.Collections;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Domain.Repositories;
public interface ITeamRepository
{
    Task GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Team team, CancellationToken cancellationToken = default);
    Task UpdateAsync(Team team, CancellationToken cancellationToken = default);
    Task<IEnumerable> GetAllAsync(CancellationToken cancellationToken = default);
}