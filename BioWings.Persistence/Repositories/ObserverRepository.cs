using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;

namespace BioWings.Persistence.Repositories;

public class ObserverRepository(AppDbContext dbContext) : GenericRepository<Observer>(dbContext), IObserverRepository
{
}

