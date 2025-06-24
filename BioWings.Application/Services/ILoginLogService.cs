using BioWings.Application.DTOs.LoginLogDtos;

namespace BioWings.Application.Services;

public interface ILoginLogService
{
    Task LogLoginAttemptAsync(LoginLogCreateDto loginLogDto, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoginLogCreateDto>> GetUserLoginHistoryAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoginLogCreateDto>> GetRecentLoginAttemptsAsync(int count = 100, CancellationToken cancellationToken = default);
}