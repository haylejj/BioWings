using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;

namespace BioWings.Persistence.Repositories;

public class SubspeciesRepository(AppDbContext dbContext) : GenericRepository<Subspecies>(dbContext), ISubspeciesRepository
{
}

