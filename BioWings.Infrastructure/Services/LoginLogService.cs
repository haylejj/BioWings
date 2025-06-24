using BioWings.Application.DTOs.LoginLogDtos;
using BioWings.Application.Interfaces;
using BioWings.Application.Services;
using BioWings.Domain.Entities;

namespace BioWings.Infrastructure.Services;

public class LoginLogService(ILoginLogRepository loginLogRepository) : ILoginLogService
{
    public async Task LogLoginAttemptAsync(LoginLogCreateDto loginLogDto, CancellationToken cancellationToken = default)
    {
        var loginLog = new LoginLog
        {
            UserId = loginLogDto.UserId,
            UserName = loginLogDto.UserName,
            IpAddress = loginLogDto.IpAddress,
            LoginDateTime = loginLogDto.LoginDateTime,
            UserAgent = loginLogDto.UserAgent,
            IsSuccessful = loginLogDto.IsSuccessful,
            FailureReason = loginLogDto.FailureReason
        };

        await loginLogRepository.CreateAsync(loginLog, cancellationToken);
    }

    public async Task<IEnumerable<LoginLogCreateDto>> GetUserLoginHistoryAsync(int userId, CancellationToken cancellationToken = default)
    {
        var loginLogs = await loginLogRepository.GetLoginLogsByUserIdAsync(userId, cancellationToken);

        return loginLogs.Select(ll => new LoginLogCreateDto
        {
            Id = ll.Id,
            UserId = ll.UserId,
            UserName = ll.UserName ?? "Bilinmeyen Kullanıcı",
            IpAddress = ll.IpAddress ?? "0.0.0.0",
            LoginDateTime = ll.LoginDateTime,
            UserAgent = ll.UserAgent ?? "Bilinmeyen Agent",
            IsSuccessful = ll.IsSuccessful,
            FailureReason = ll.FailureReason
        });
    }

    public async Task<IEnumerable<LoginLogCreateDto>> GetRecentLoginAttemptsAsync(int count = 100, CancellationToken cancellationToken = default)
    {
        var loginLogs = await loginLogRepository.GetRecentLoginLogsAsync(count, cancellationToken);

        return loginLogs.Select(ll => new LoginLogCreateDto
        {
            Id = ll.Id,
            UserId = ll.UserId,
            UserName = ll.UserName ?? "Bilinmeyen Kullanıcı",
            IpAddress = ll.IpAddress ?? "0.0.0.0",
            LoginDateTime = ll.LoginDateTime,
            UserAgent = ll.UserAgent ?? "Bilinmeyen Agent",
            IsSuccessful = ll.IsSuccessful,
            FailureReason = ll.FailureReason
        });
    }
}