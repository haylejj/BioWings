using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;

public class ObserverRepository(AppDbContext dbContext) : GenericRepository<Observer>(dbContext), IObserverRepository
{
    public async Task<Observer?> GetByFullNameAsync(string fullName, CancellationToken cancellationToken = default) => await _dbSet.AsNoTracking().Where(x => x.FullName == fullName).FirstOrDefaultAsync(cancellationToken);
    public async Task<Observer?> GetByNameAndSurnameAsync(string name, string surname, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(x => x.Name == name && x.Surname == surname, cancellationToken);

}

