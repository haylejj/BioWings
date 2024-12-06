using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;

namespace BioWings.Persistence.Repositories;

public class GenusRepository(AppDbContext dbContext) : GenericRepository<Genus>(dbContext), IGenusRepository
{
}

