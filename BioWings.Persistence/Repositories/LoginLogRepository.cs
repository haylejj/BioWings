using BioWings.Application.Interfaces;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;

public class LoginLogRepository : ILoginLogRepository
{
    private readonly AppDbContext _context;

    public LoginLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(LoginLog loginLog, CancellationToken cancellationToken = default)
    {
        await _context.LoginLogs.AddAsync(loginLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return loginLog.Id;
    }

    public async Task<IEnumerable<LoginLog>> GetLoginLogsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.LoginLogs
            .Where(ll => ll.UserId == userId)
            .Include(ll => ll.User)
            .OrderByDescending(ll => ll.LoginDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LoginLog>> GetRecentLoginLogsAsync(int count = 100, CancellationToken cancellationToken = default)
    {
        return await _context.LoginLogs
            .Include(ll => ll.User)
            .OrderByDescending(ll => ll.LoginDateTime)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LoginLog>> GetLoginLogsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.LoginLogs
            .Where(ll => ll.LoginDateTime >= startDate && ll.LoginDateTime <= endDate)
            .Include(ll => ll.User)
            .OrderByDescending(ll => ll.LoginDateTime)
            .ToListAsync(cancellationToken);
    }
} 