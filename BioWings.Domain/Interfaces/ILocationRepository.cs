using BioWings.Domain.Entities;

namespace BioWings.Domain.Interfaces;

public interface ILocationRepository : IGenericRepository<Location>
{
    Task<Location?> GetByIdWithProvinceAsync(int id, CancellationToken cancellationToken = default);
}


