using BioWings.Domain.Entities;
using BioWings.Domain.Enums;
using System.Linq.Expressions;

namespace BioWings.Application.Interfaces;

public interface ILocationRepository : IGenericRepository<Location>
{
    Task<Location?> GetByIdWithProvinceAsync(int id, CancellationToken cancellationToken = default);
    Task<Location?> FirstOrDefaultAsync(Expression<Func<Location, bool>> predicate, CancellationToken cancellationToken = default);
    IQueryable<Location?> GetUnusedLocationRecord();
    Task<Location?> GetByCoordinatesAsync(decimal latitude, decimal longitude, string squareRef, CoordinatePrecisionLevel coordinatePrecisionLevel, CancellationToken cancellationToken = default);
    Task<Location?> GetOrCreateLocationAsync<TDto>(TDto dto, int? provinceId, CancellationToken cancellationToken) where TDto : ILocationImportDto;
}


