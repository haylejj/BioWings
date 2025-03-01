using BioWings.Application.Features.Commands.UserCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.UserHandlers.Write;
public class UserCreateCommandHandler(IUserRepository userRepository, ILogger<UserCreateCommandHandler> logger, IPasswordHashService passwordHashService, IUnitOfWork unitOfWork, IEmailService emailService) : IRequestHandler<UserCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(UserCreateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Email kontrolü
            var existingUser = await userRepository.IsExistByEmailAsync(request.Email, cancellationToken);
            if (existingUser)
            {
                logger.LogWarning("User creation failed: Email {Email} already exists", request.Email);
                return ServiceResult.Error("Email already exists");
            }

            // Validasyon
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                logger.LogWarning("User creation failed: Password is required");
                return ServiceResult.Error("Password is required");
            }

            // Yeni kullanıcı oluşturma
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHashService.HashPassword(request.Password),
                EmailConfirmed = false,
            };

            // Email doğrulama token'ını oluşturma (eğer email onaylanmamışsa)
            if (!request.IsEmailConfirmed)
            {
                user.EmailConfirmationToken = Guid.NewGuid().ToString();
                user.EmailConfirmationTokenExpireDate = DateTime.UtcNow.AddDays(1);

                // Email doğrulama maili gönderme
                await emailService.SendEmailAsync(
                    user.Email,
                    "Email Doğrulama",
                    $"Hesabınızı doğrulamak için aşağıdaki linke tıklayın: " +
                    $"<a href='https://localhost:7128/confirm-email?token={user.EmailConfirmationToken}&email={user.Email}'>Hesabımı Doğrula</a>"
                );
            }

            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation(
                "User created successfully. ID: {UserId}, Email: {Email}",
                user.Id,
                user.Email);

            return ServiceResult.SuccessAsCreated($"/api/User/{user.Id}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error occurred while creating user. Email: {Email}",
                request.Email);

            return ServiceResult.Error("Failed to create user: " + ex.Message);
        }
    }
}
