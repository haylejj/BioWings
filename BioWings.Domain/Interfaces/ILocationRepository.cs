using BioWings.Domain.Entities;
using System.Linq.Expressions;

namespace BioWings.Domain.Interfaces;

public interface ILocationRepository : IGenericRepository<Location>
{
    Task<Location?> GetByIdWithProvinceAsync(int id, CancellationToken cancellationToken = default);
    Task<Location?> FirstOrDefaultAsync(Expression<Func<Location, bool>> predicate, CancellationToken cancellationToken = default);
    IQueryable<Location?> GetUnusedLocationRecord();
    Task<Location?> GetByCoordinatesAsync(decimal latitude, decimal longitude, string squareRef, CancellationToken cancellationToken = default);
}


