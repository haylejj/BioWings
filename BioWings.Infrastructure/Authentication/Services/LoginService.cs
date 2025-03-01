using BioWings.Application.DTOs.LoginDtos;
using BioWings.Application.Interfaces;
using BioWings.Application.Services;
using Microsoft.Extensions.Logging;

namespace BioWings.Infrastructure.Authentication.Services;
public class LoginService(IUserRepository userRepository, ILogger<LoginService> logger, IPasswordHashService passwordHashService) : ILoginService
{
    public async Task<bool> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(loginDto.Email, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("Login failed for email {Email}", loginDto.Email);
            return false;
        }
        if (!await userRepository.IsEmailConfirmedAsync(loginDto.Email, cancellationToken))
        {
            logger.LogWarning("Login failed for email {Email}", loginDto.Email);
            return false;
        }
        if (!passwordHashService.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            logger.LogWarning("Login failed for email {Email}", loginDto.Email);
            return false;
        }
        logger.LogInformation("User with email {Email} logged in", loginDto.Email);
        return true;
    }
}
