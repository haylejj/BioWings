using BioWings.Application.Features.Commands.EmailCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.EmailHandlers;
public class ResendEmailConfirmMailCommandHandler(IUserRepository userRepository, IEmailService emailService, IEncryptionService encryptionService, IUnitOfWork unitOfWork, ILogger<ResendEmailConfirmMailCommandHandler> logger) : IRequestHandler<ResendEmailConfirmMailCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ResendEmailConfirmMailCommand request, CancellationToken cancellationToken)
    {

        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User with email {Email} not found", request.Email);
            return ServiceResult.Error($"User with email {request.Email} not found");
        }
        if (user.EmailConfirmed)
        {
            logger.LogWarning("User with email {Email} already confirmed", request.Email);
            return ServiceResult.Error($"User with email {request.Email} already confirmed");
        }
        var token = Guid.NewGuid().ToString();
        user.EmailConfirmationToken = token;
        user.EmailConfirmationTokenExpireDate = DateTime.UtcNow.AddDays(1);
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        var codedToken = $"{request.Email}|{token}";
        var encodedToken = encryptionService.Encrypt(codedToken);
        var url = $"https://localhost:7269/Email/EmailConfirm?code={encodedToken}";
        var subject = "BioWings - Email Doğrulama";
        var body = $"<html><body><h2>Merhaba {user.FirstName + user.LastName},</h2><p>Hesabınızı doğrulamak için aşağıdaki bağlantıya tıklayınız:</p><p><a href='{url}'>Hesabımı Doğrula</a></p><p>Bu bağlantı 1 gün geçerlidir.</p><p>Eğer siz hesap doğrulama talebinde bulunmadıysanız, bu e-postayı dikkate almayınız.</p><p>Saygılarımızla,<br>BioWings Ekibi</p></body></html>";
        await emailService.SendEmailAsync(request.Email, subject, body);
        logger.LogInformation("Email confirmation mail sent to {Email}", request.Email);
        return ServiceResult.Success();
    }
}
