using BioWings.Domain.Entities;

namespace BioWings.Application.Interfaces;

public interface ILoginLogRepository
{
    Task<int> CreateAsync(LoginLog loginLog, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoginLog>> GetLoginLogsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoginLog>> GetRecentLoginLogsAsync(int count = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoginLog>> GetLoginLogsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}