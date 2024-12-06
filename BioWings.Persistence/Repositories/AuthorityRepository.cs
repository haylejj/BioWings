using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;

namespace BioWings.Persistence.Repositories;
public class AuthorityRepository(AppDbContext dbContext) : GenericRepository<Authority>(dbContext), IAuthorityRepository
{
}
