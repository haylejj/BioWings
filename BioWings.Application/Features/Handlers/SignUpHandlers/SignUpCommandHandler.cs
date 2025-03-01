using BioWings.Application.Features.Commands.SignUpCommands;
using BioWings.Application.Features.Results.SignUpResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SignUpHandlers;
public class SignUpCommandHandler(IUserRepository userRepository, ILogger<SignUpCommandHandler> logger, IPasswordHashService passwordHashService, IEncryptionService encryptionService, IUnitOfWork unitOfWork, IEmailService emailService) : IRequestHandler<SignUpCommand, ServiceResult<SignUpResult>>
{
    public async Task<ServiceResult<SignUpResult>> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Begin processing SignUp request for email: {Email}", request.Email);

        if (request.Password != request.ConfirmPassword)
        {
            logger.LogWarning("Password mismatch during signup for email: {Email}", request.Email);
            return ServiceResult<SignUpResult>.Error("Password and ConfirmPassword do not match");
        }

        var isExist = await userRepository.IsExistByEmailAsync(request.Email, cancellationToken);
        if (isExist)
        {
            // kullanıcı var ama email doğrulanmamışsa tekrar mail gönderme işlemi  yapılacak.
            if (await userRepository.IsEmailConfirmedAsync(request.Email, cancellationToken))
            {
                logger.LogWarning("Signup failed: Email already exists: {Email}", request.Email);
                return ServiceResult<SignUpResult>.Error("Email already exists");
            }
            var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
            user!.EmailConfirmationToken = Guid.NewGuid().ToString();
            user.EmailConfirmationTokenExpireDate = DateTime.UtcNow.AddDays(1);
            user.CountryId = request.CountryId;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PasswordHash = passwordHashService.HashPassword(request.Password);
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            // Email doğrulama maili gönderme
            var combinedData = $"{user.Email}|{user.EmailConfirmationToken}";
            var encryptedCode = encryptionService.Encrypt(combinedData);
            await emailService.SendEmailAsync(
                user.Email,
                "Email Confirmation",
                $"Please click the link below to confirm your account: " +
                $"<a href='https://localhost:7269/Email/EmailConfirm?code={encryptedCode}'>Confirm my account</a>"
            );
            logger.LogInformation("Confirmation email sent to: {Email}", user.Email);
            logger.LogInformation("User previously attempted registration but didn't confirm their email. Confirmation has been resent.");
            return ServiceResult<SignUpResult>.Success(new SignUpResult { IsSuccess=true, Message="" });
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            logger.LogWarning("Signup failed: Password is required for email: {Email}", request.Email);
            return ServiceResult<SignUpResult>.Error("Password is required");
        }

        try
        {
            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = passwordHashService.HashPassword(request.ConfirmPassword),
                CountryId = request.CountryId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = false,
                EmailConfirmationToken = Guid.NewGuid().ToString(),
                EmailConfirmationTokenExpireDate = DateTime.UtcNow.AddDays(1)
            };

            logger.LogDebug("Created user object with email confirmation token expiring at: {ExpireDate}", newUser.EmailConfirmationTokenExpireDate);



            await userRepository.AddAsync(newUser, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User successfully created. UserID: {UserId}, Email: {Email}, FirstName: {FirstName}",
                newUser.Id, newUser.Email, newUser.FirstName);

            // Email doğrulama maili gönderme
            var combinedData = $"{newUser.Email}|{newUser.EmailConfirmationToken}";
            var encryptedCode = encryptionService.Encrypt(combinedData);
            await emailService.SendEmailAsync(
                newUser.Email,
                "Email Confirmation",
                $"Please click the link below to confirm your account: " +
                $"<a href='https://localhost:7269/Email/EmailConfirm?code={encryptedCode}'>Confirm my account</a>"
            );

            logger.LogInformation("Confirmation email sent to: {Email}", newUser.Email);

            return ServiceResult<SignUpResult>.SuccessAsCreated(new SignUpResult { IsSuccess=true, Message="" }, $"api/SignUp/{newUser.Id}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during user creation for email: {Email}. Error: {ErrorMessage}",
                request.Email, ex.Message);
            return ServiceResult<SignUpResult>.Error($"User creation failed: {ex.Message}");
        }
    }
}
