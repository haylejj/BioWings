using BioWings.Application.DTOs.LoginDtos;
using BioWings.Application.Features.Commands.LoginCommands;
using BioWings.Application.Features.Results.LoginResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.LoginHandlers;
public class LoginCommandHandler(ILoginService loginService, ILogger<LoginCommandHandler> logger, ITokenService tokenService, IUserRoleRepository userRoleRepository, IUserRepository userRepository) : IRequestHandler<LoginCommand, ServiceResult<LoginResult>>
{
    public async Task<ServiceResult<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await loginService.LoginAsync(new LoginDto { Email = request.Email, Password = request.Password }, cancellationToken);
        if (result)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
            {
                logger.LogWarning("User not found with email {Email} after successful login", request.Email);
                return ServiceResult<LoginResult>.Error("User not found");
            }

            var userRoles = await userRoleRepository.GetUserRolesByUserIdAsync(user.Id, cancellationToken);
            var token = tokenService.CreateToken(user, userRoles);
            logger.LogInformation("User with email {Email} logged in", request.Email);
            return ServiceResult<LoginResult>.Success(new LoginResult { Token = token.AccessToken, RefreshToken = token.RefreshToken, Expiration = token.Expiration, Roles = userRoles.Select(x => x.RoleName).ToList() });
        }
        logger.LogWarning("Login failed for user with email {Email}", request.Email);
        return ServiceResult<LoginResult>.Error("Invalid email or password");
    }
}
