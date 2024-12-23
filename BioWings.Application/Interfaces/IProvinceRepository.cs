using BioWings.Domain.Entities;

namespace BioWings.Application.Interfaces;

public interface IProvinceRepository : IGenericRepository<Province>
{
    Task<Province?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Province?> GetByProvinceCodeAsync(int code, CancellationToken cancellationToken = default);
    Task<Province?> GetByNameOrCodeAsync(string name, int? code, CancellationToken cancellationToken = default);
}


