using BioWings.Application.Features.Commands.EmailCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.EmailHandlers;
public class ResetPasswordCommandHandler(ILogger<ResetPasswordCommandHandler> logger, IUnitOfWork unitOfWork, IUserRepository userRepository, IEncryptionService encryptionService, IPasswordHashService passwordHashService) : IRequestHandler<ResetPasswordCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var decodedToken = encryptionService.Decrypt(request.Token);
        var tokenParts = decodedToken.Split('|');
        var email = tokenParts[0];
        var token = tokenParts[1];
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User with email {Email} not found", email);
            return ServiceResult.Error($"User with email {email} not found");
        }
        if (user.ResetPasswordToken != token)
        {
            logger.LogWarning("Invalid token for user with email {Email}", email);
            return ServiceResult.Error("Invalid token");
        }
        if (user.ResetPasswordTokenExpireDate < DateTime.UtcNow)
        {
            logger.LogWarning("Token expired for user with email {Email}", email);
            return ServiceResult.Error("Token expired");
        }
        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpireDate = null;
        user.PasswordHash = passwordHashService.HashPassword(request.Password);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Password changed for user with email {Email}", email);
        return ServiceResult.Success();
    }
}
