using BioWings.Application.Features.Commands.UserCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.UserHandlers.Write;

public class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHashService passwordHashService,
    IUnitOfWork unitOfWork,
    IValidator<ChangePasswordCommand> validator,
    ILogger<ChangePasswordCommandHandler> logger) : IRequestHandler<ChangePasswordCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // FluentValidation
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                logger.LogWarning("Password change validation failed: {Errors}", string.Join(", ", errorMessages));
                return ServiceResult.Error(errorMessages);
            }

            // Kullanıcıyı bul
            var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                logger.LogWarning("User with ID {UserId} not found", request.UserId);
                return ServiceResult.Error("Kullanıcı bulunamadı");
            }

            // Mevcut şifreyi kontrol et
            if (!passwordHashService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                logger.LogWarning("Current password verification failed for user {UserId}", request.UserId);
                return ServiceResult.Error("Mevcut şifre yanlış");
            }

            // Şifreyi güncelle
            user.PasswordHash = passwordHashService.HashPassword(request.NewPassword);
            
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Password changed successfully for user {UserId}", request.UserId);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error changing password for user {UserId}", request.UserId);
            return ServiceResult.Error("Şifre değiştirme sırasında bir hata oluştu");
        }
    }
} 