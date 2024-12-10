using BioWings.Domain.Entities;

namespace BioWings.Domain.Interfaces;

public interface IProvinceRepository : IGenericRepository<Province>
{
    Task<Province?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}


