using BioWings.Application.Interfaces;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;
public class UserRepository(AppDbContext dbContext) : GenericRepository<User>(dbContext), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<bool> IsEmailConfirmedAsync(string email, CancellationToken cancellationToken = default) => await _dbSet.AnyAsync(u => u.Email == email && u.EmailConfirmed, cancellationToken);
    public async Task<bool> IsExistByEmailAsync(string email, CancellationToken cancellationToken = default) => await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
}
