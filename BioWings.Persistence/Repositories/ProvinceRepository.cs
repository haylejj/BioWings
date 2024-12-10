using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;

public class ProvinceRepository(AppDbContext dbContext) : GenericRepository<Province>(dbContext), IProvinceRepository
{
    public async Task<Province?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
}

