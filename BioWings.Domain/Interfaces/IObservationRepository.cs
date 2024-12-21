using BioWings.Domain.Entities;
using System.Linq.Expressions;

namespace BioWings.Domain.Interfaces;

public interface IObservationRepository : IGenericRepository<Observation>
{
    Task<Observation?> FirstOrDefaultAsync(Expression<Func<Observation, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Observation?> GetByIdWithAllNavigationsAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Observation>> GetAllDuplicateObservationsAsync(CancellationToken cancellationToken = default);
    Task RemoveDuplicatesAsync(List<int> batchIds, CancellationToken cancellationToken = default);
    Task<List<int>> GetAllDuplicateObservationIdsAsync(CancellationToken cancellationToken = default);
}


