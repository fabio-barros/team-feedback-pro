using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Application.Common.Abstractions;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Team>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Team> AddAsync(Team team, CancellationToken cancellationToken = default);
    Task UpdateAsync(Team team, CancellationToken cancellationToken = default);
    Task DeleteAsync(Team team, CancellationToken cancellationToken = default);
}