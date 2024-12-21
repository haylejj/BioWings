using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;

public class GenusRepository(AppDbContext dbContext) : GenericRepository<Genus>(dbContext), IGenusRepository
{
    public async Task<Genus?> GetByNameAndFamilyIdAsync(string name, int? familyId, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(g => g.Name == name && g.FamilyId == familyId, cancellationToken);

    public async Task<Genus?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(g => g.Name == name, cancellationToken);
}

