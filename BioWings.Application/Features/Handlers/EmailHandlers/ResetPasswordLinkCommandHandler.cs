using BioWings.Application.Features.Commands.EmailCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.EmailHandlers;
public class ResetPasswordLinkCommandHandler(IUserRepository userRepository, IEmailService emailService, ILogger<ResetPasswordLinkCommandHandler> logger, IUnitOfWork unitOfWork, IEncryptionService encryptionService) : IRequestHandler<ResetPasswordLinkCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ResetPasswordLinkCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User with email {Email} not found", request.Email);
            return ServiceResult.Error($"User with email {request.Email} not found");
        }
        var token = Guid.NewGuid().ToString();
        user.ResetPasswordToken = token;
        user.ResetPasswordTokenExpireDate = DateTime.UtcNow.AddHours(1);
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        var codedToken = $"{request.Email}|{token}";
        var encodedToken = encryptionService.Encrypt(codedToken);
        var url = $"https://localhost:7269/Password/ChangePassword?token={encodedToken}";
        var subject = "BioWings - Şifre Sıfırlama";
        var body = $@"
            <html>
            <body>
                <h2>Merhaba {user.FirstName + user.LastName},</h2>
                <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayınız:</p>
                <p><a href='{url}'>Şifremi Sıfırla</a></p>
                <p>Bu bağlantı 1 saat geçerlidir.</p>
                <p>Eğer siz şifre sıfırlama talebinde bulunmadıysanız, bu e-postayı dikkate almayınız.</p>
                <p>Saygılarımızla,<br>BioWings Ekibi</p>
            </body>
            </html>";
        await emailService.SendEmailAsync(request.Email, subject, body);
        logger.LogInformation("Reset password link sent to {Email}", request.Email);
        return ServiceResult.Success();
    }
}
