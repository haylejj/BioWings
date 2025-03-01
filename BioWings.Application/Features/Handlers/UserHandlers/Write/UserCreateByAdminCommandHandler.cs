using BioWings.Application.Features.Commands.UserCommands;
using BioWings.Application.Helper;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.UserHandlers.Write;
public class UserCreateByAdminCommandHandler(IEmailService emailService, IPasswordHashService passwordHashService, IEncryptionService encryptionService, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<UserCreateByAdminCommandHandler> logger) : IRequestHandler<UserCreateByAdminCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(UserCreateByAdminCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
            {
                logger.LogError("User creation failed: Request object is null");
                return ServiceResult.Error("Invalid request data", System.Net.HttpStatusCode.BadRequest);
            }

            logger.LogInformation("Processing user creation request for email: {Email}", request.Email);

            try
            {
                bool emailExists = await userRepository.IsExistByEmailAsync(request.Email, cancellationToken);
                if (emailExists)
                {
                    logger.LogWarning("User creation failed: Email {Email} already exists in the system", request.Email);
                    return ServiceResult.Error("A user with this email already exists", System.Net.HttpStatusCode.Conflict);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking email existence for {Email}", request.Email);
                return ServiceResult.Error("Error validating email uniqueness", System.Net.HttpStatusCode.InternalServerError);
            }

            string userPassword = RandomPasswordHelper.GenerateRandomPassword(8);

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CountryId = request.CountryId,
                PasswordHash = passwordHashService.HashPassword(userPassword),
                EmailConfirmed = false,
            };

            try
            {
                await userRepository.AddAsync(user, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                logger.LogInformation("User successfully created with ID: {UserId}, Email: {Email}",
                    user.Id, user.Email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database error occurred while creating user with email: {Email}", request.Email);
                return ServiceResult.Error("Failed to create user in the database", System.Net.HttpStatusCode.InternalServerError);
            }
            try
            {
                // Send email confirmation mail
                var token = Guid.NewGuid().ToString();
                var codedToken = $"{user.Email}|{token}";
                var encodedToken = encryptionService.Encrypt(codedToken);
                var url = $"https://localhost:7269/Email/EmailConfirm?code={encodedToken}";


                logger.LogInformation("Email confirmation mail sent to {Email}", request.Email);
                string emailSubject = "Welcome to Our Platform - Your Account Information";
                string emailBody = $@"
                    <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 5px;"">
                        <h2 style=""color: #3f51b5;"">Hoş Geldiniz!</h2>
                        <p>Hesabınız bir yönetici tarafından oluşturuldu. Giriş bilgileriniz:</p>
    
                        <div style=""background-color: #f5f5f5; padding: 15px; border-radius: 4px; margin: 15px 0;"">
                            <p><strong>E-posta:</strong> {request.Email}</p>
                            <p><strong>Şifre:</strong> {userPassword}</p>
                            <p><strong>Önemli:</strong> E-posta adresinizi doğrulamanız gerekiyor. Doğrulama yapılmadan giriş yapamazsınız.</p>
 
                        </div>
                            <a href=""{url}"" style=""background-color: #4caf50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px; display: inline-block;"">E-posta Adresimi Doğrula</a>
                        
    
                        <p style=""font-size: 14px; color: #757575;"">Bu doğrulama bağlantısı <strong>24 saat</strong> geçerlidir. Güvenliğiniz için ilk girişte şifrenizi değiştirin.</p>
                    </div>    
                ";

                await emailService.SendEmailAsync(request.Email, emailSubject, emailBody);

                // Save the token to the database
                user.EmailConfirmationToken = token;
                user.EmailConfirmationTokenExpireDate = DateTime.Now.AddDays(1);
                // Update the user
                userRepository.Update(user);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Welcome email sent to new user: {Email}", request.Email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send welcome email to user: {Email}", request.Email);
            }

            return ServiceResult.SuccessAsCreated($"api/Users/ByAdmin/{user.Id}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred while creating user with email: {Email}",
                request?.Email ?? "unknown");
            return ServiceResult.Error("An unexpected error occurred", System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
