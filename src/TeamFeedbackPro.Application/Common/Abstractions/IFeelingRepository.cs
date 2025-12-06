using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Application.Common.Abstractions;

public interface IFeelingRepository
{
    Task<IEnumerable<Feeling>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Feeling?> GetByIdAsync(Guid feelingId, CancellationToken cancellationToken = default);

}