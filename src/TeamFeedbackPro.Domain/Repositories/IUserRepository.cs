using System.Collections;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Domain.Repositories;
public interface IUserRepository
{
    Task GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<IEnumerable> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default);
}