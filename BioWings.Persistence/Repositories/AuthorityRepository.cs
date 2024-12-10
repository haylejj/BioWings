using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;
public class AuthorityRepository(AppDbContext dbContext) : GenericRepository<Authority>(dbContext), IAuthorityRepository
{
    public async Task<Authority?> GetByNameAndYearAsync(string name, int year, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(a => a.Name == name && a.Year == year, cancellationToken);
}
