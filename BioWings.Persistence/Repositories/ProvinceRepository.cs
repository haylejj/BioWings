using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;

namespace BioWings.Persistence.Repositories;

public class ProvinceRepository(AppDbContext dbContext) : GenericRepository<Province>(dbContext), IProvinceRepository
{
}

