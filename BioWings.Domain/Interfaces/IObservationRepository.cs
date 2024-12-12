using BioWings.Domain.Entities;
using System.Linq.Expressions;

namespace BioWings.Domain.Interfaces;

public interface IObservationRepository : IGenericRepository<Observation>
{
    Task<Observation?> FirstOrDefaultAsync(Expression<Func<Observation, bool>> predicate, CancellationToken cancellationToken = default);
}


