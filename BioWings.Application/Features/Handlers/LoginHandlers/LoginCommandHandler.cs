using BioWings.Application.DTOs.LoginDtos;
using BioWings.Application.DTOs.LoginLogDtos;
using BioWings.Application.Features.Commands.LoginCommands;
using BioWings.Application.Features.Results.LoginResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.LoginHandlers;
public class LoginCommandHandler(ILoginService loginService, ILogger<LoginCommandHandler> logger, ITokenService tokenService, IUserRoleRepository userRoleRepository, IUserRepository userRepository, ILoginLogService loginLogService) : IRequestHandler<LoginCommand, ServiceResult<LoginResult>>
{
    public async Task<ServiceResult<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        var loginDateTime = DateTime.Now;
        
        // Kullanıcı yoksa önce log tut, sonra login deneme
        if (user == null)
        {
            // Kullanıcı bulunamadı log'u
            await loginLogService.LogLoginAttemptAsync(new LoginLogCreateDto
            {
                UserId = -1, // Geçici çözüm: var olmayan kullanıcı için -1
                UserName = request.Email,
                IpAddress = request.IpAddress ?? "Unknown",
                LoginDateTime = loginDateTime,
                UserAgent = request.UserAgent ?? "Unknown",
                IsSuccessful = false,
                FailureReason = "User not found"
            }, cancellationToken);
            
            logger.LogWarning("Login failed - User not found with email {Email}", request.Email);
            return ServiceResult<LoginResult>.Error("Invalid email or password");
        }
        
        // Kullanıcı varsa şifre kontrolü yap
        var result = await loginService.LoginAsync(new LoginDto { Email = request.Email, Password = request.Password }, cancellationToken);
        if (result)
        {
            var userRoles = await userRoleRepository.GetUserRolesByUserIdAsync(user.Id, cancellationToken);
            var token = tokenService.CreateToken(user, userRoles);
            
            // Başarılı giriş log'u
            await loginLogService.LogLoginAttemptAsync(new LoginLogCreateDto
            {
                UserId = user.Id,
                UserName = $"{user.FirstName} {user.LastName}".Trim(),
                IpAddress = request.IpAddress ?? "Unknown",
                LoginDateTime = loginDateTime,
                UserAgent = request.UserAgent ?? "Unknown",
                IsSuccessful = true,
                FailureReason = null
            }, cancellationToken);
            
            logger.LogInformation("User with email {Email} logged in successfully", request.Email);
            return ServiceResult<LoginResult>.Success(new LoginResult { Token = token.AccessToken, RefreshToken = token.RefreshToken, Expiration = token.Expiration, Roles = userRoles.Select(x => x.RoleName).ToList() });
        }
        
        // Başarısız giriş log'u - geçersiz şifre
        await loginLogService.LogLoginAttemptAsync(new LoginLogCreateDto
        {
            UserId = user.Id,
            UserName = $"{user.FirstName} {user.LastName}".Trim(),
            IpAddress = request.IpAddress ?? "Unknown",
            LoginDateTime = loginDateTime,
            UserAgent = request.UserAgent ?? "Unknown",
            IsSuccessful = false,
            FailureReason = "Invalid password"
        }, cancellationToken);
        
        logger.LogWarning("Login failed - Invalid password for user with email {Email}", request.Email);
        return ServiceResult<LoginResult>.Error("Invalid email or password");
    }
}
