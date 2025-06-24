using BioWings.Application.DTOs.LoginLogDtos;
using BioWings.Application.Interfaces;
using BioWings.Application.Services;
using BioWings.Domain.Entities;

namespace BioWings.Infrastructure.Services;

public class LoginLogService : ILoginLogService
{
    private readonly ILoginLogRepository _loginLogRepository;

    public LoginLogService(ILoginLogRepository loginLogRepository)
    {
        _loginLogRepository = loginLogRepository;
    }

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

        await _loginLogRepository.CreateAsync(loginLog, cancellationToken);
    }

    public async Task<IEnumerable<LoginLogCreateDto>> GetUserLoginHistoryAsync(int userId, CancellationToken cancellationToken = default)
    {
        var loginLogs = await _loginLogRepository.GetLoginLogsByUserIdAsync(userId, cancellationToken);
        
        return loginLogs.Select(ll => new LoginLogCreateDto
        {
            UserId = ll.UserId,
            UserName = ll.UserName,
            IpAddress = ll.IpAddress,
            LoginDateTime = ll.LoginDateTime,
            UserAgent = ll.UserAgent,
            IsSuccessful = ll.IsSuccessful,
            FailureReason = ll.FailureReason
        });
    }

    public async Task<IEnumerable<LoginLogCreateDto>> GetRecentLoginAttemptsAsync(int count = 100, CancellationToken cancellationToken = default)
    {
        var loginLogs = await _loginLogRepository.GetRecentLoginLogsAsync(count, cancellationToken);
        
        return loginLogs.Select(ll => new LoginLogCreateDto
        {
            UserId = ll.UserId,
            UserName = ll.UserName,
            IpAddress = ll.IpAddress,
            LoginDateTime = ll.LoginDateTime,
            UserAgent = ll.UserAgent,
            IsSuccessful = ll.IsSuccessful,
            FailureReason = ll.FailureReason
        });
    }
} 