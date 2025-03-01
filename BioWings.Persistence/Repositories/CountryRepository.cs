using BioWings.Application.Interfaces;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;

namespace BioWings.Persistence.Repositories;
public class CountryRepository(AppDbContext dbContext) : GenericRepository<Country>(dbContext), ICountryRepository
{
}
