using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;

namespace BioWings.Persistence.Repositories;

public class SpeciesTypeRepository(AppDbContext dbContext) : GenericRepository<SpeciesType>(dbContext), ISpeciesTypeRepository
{
}

