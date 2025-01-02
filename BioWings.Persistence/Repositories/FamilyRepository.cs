using BioWings.Application.DTOs.StatisticsDtos;
using BioWings.Application.Interfaces;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;

public class FamilyRepository(AppDbContext dbContext) : GenericRepository<Family>(dbContext), IFamilyRepository
{
    public async Task<Family?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => await _dbSet.AsNoTracking().Where(x => x.Name==name).FirstOrDefaultAsync(cancellationToken);

    public async Task<List<FamilyStatDto>> GetTopFamiliesAsync(CancellationToken cancellationToken = default) => await _dbSet
        .AsNoTracking()
        .Include(x=> x.Genera).ThenInclude(y => y.Species).ThenInclude(z => z.Observations)
        .Select(x => new FamilyStatDto
        {
            ObservationCount = x.Genera.Sum(y=> y.Species.Sum(z => z.Observations.Count)),
            FamilyName = x.Name,
            SpeciesCount = x.Genera.Sum(y => y.Species.Count)
        })
        .OrderByDescending(x => x.SpeciesCount)
        .Take(5)
        .ToListAsync(cancellationToken);
}

