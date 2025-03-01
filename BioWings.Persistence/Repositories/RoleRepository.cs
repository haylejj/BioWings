using BioWings.Application.Interfaces;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;

namespace BioWings.Persistence.Repositories;
public class RoleRepository(AppDbContext dbContext) : GenericRepository<Role>(dbContext), IRoleRepository
{
}
