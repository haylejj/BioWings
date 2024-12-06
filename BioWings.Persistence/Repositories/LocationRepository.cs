using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;

namespace BioWings.Persistence.Repositories;

public class LocationRepository(AppDbContext dbContext) : GenericRepository<Location>(dbContext), ILocationRepository
{
}

