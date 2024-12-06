using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;

namespace BioWings.Persistence.Repositories;

public class ObservationRepository(AppDbContext dbContext) : GenericRepository<Observation>(dbContext), IObservationRepository
{
}

