using BioWings.Application.Interfaces;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;
using System.Linq.Expressions;

namespace BioWings.Persistence.Repositories;

public class LocationRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : GenericRepository<Location>(dbContext), ILocationRepository
{
    private readonly DbContext dbContext1 = unitOfWork.GetContext();
    public async Task<Location?> FirstOrDefaultAsync(Expression<Func<Location, bool>> predicate, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<Location?> GetByCoordinatesAsync(decimal latitude, decimal longitude, string squareRef, CancellationToken cancellationToken = default)
    {
        latitude = Math.Round(latitude, 6);
        longitude = Math.Round(longitude, 6);
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(squareRef))
        {
            query = query.Where(l => l.SquareRef == squareRef);
        }
        return query.Where(l => l.Latitude == latitude && l.Longitude == longitude)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Location?> GetByIdWithProvinceAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.Include(x => x.Province).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Location?> GetOrCreateLocationAsync<TDto>(TDto dto, int? provinceId, CancellationToken cancellationToken) where TDto : ILocationImportDto
    {
        var latitude = Math.Round(dto.Latitude, 6);
        var longitude = Math.Round(dto.Longitude, 6);
        var parameters = new[]
        {
        new MySqlParameter("@p0", (object)provinceId ?? DBNull.Value),
        new MySqlParameter("@p1", (object)dto.SquareRef ?? DBNull.Value),
        new MySqlParameter("@p2", latitude),
        new MySqlParameter("@p3", longitude),
        new MySqlParameter("@p4", dto.SquareLatitude),
        new MySqlParameter("@p5", dto.SquareLongitude),
        new MySqlParameter("@p6", (object)dto.DecimalDegrees ?? DBNull.Value),
        new MySqlParameter("@p7", (object)dto.DegreesMinutesSeconds ?? DBNull.Value),
        new MySqlParameter("@p8", (object)dto.DecimalMinutes ?? DBNull.Value),
        new MySqlParameter("@p9", (object)dto.UtmCoordinates ?? DBNull.Value),
        new MySqlParameter("@p10", (object)dto.MgrsCoordinates ?? DBNull.Value),
        new MySqlParameter("@p11", dto.Altitude1),
        new MySqlParameter("@p12", dto.Altitude2),
        new MySqlParameter("@p13", (object)dto.UtmReference ?? DBNull.Value),
        new MySqlParameter("@p14", (int)dto.CoordinatePrecisionLevel)
    };

        var sql = "CALL sp_GetOrCreateLocation(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14)";

        List<Location> result = await dbContext1.Set<Location>()
            .FromSqlRaw(sql, parameters)
            .ToListAsync(cancellationToken);

        Location? location = result.FirstOrDefault();
        if (location != null)
        {
            return location;
        }
        return null;
    }

    public IQueryable<Location?> GetUnusedLocationRecord() => _dbSet.Include(x => x.Observations).Where(loc => !loc.Observations.Any());
}

