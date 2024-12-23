using BioWings.Application.Interfaces;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;
public class AuthorityRepository(AppDbContext dbContext) : GenericRepository<Authority>(dbContext), IAuthorityRepository
{
    public async Task<Authority?> GetByNameAndYearAsync(string? name, int? year, CancellationToken cancellationToken = default) => await _dbSet.AsNoTracking().Where(x => x.Name==name && x.Year==year).FirstOrDefaultAsync(cancellationToken);
}
