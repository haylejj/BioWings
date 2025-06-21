using BioWings.Application.Features.Commands.EmailCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BioWings.Application.Features.Handlers.EmailHandlers;
public class ResetPasswordLinkCommandHandler(IUserRepository userRepository, IEmailService emailService, ILogger<ResetPasswordLinkCommandHandler> logger, IUnitOfWork unitOfWork, IEncryptionService encryptionService, IOptions<ApiSettings> apiSettings) : IRequestHandler<ResetPasswordLinkCommand, ServiceResult>
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
        var url = $"{apiSettings.Value.FrontendUrl}/password/change?token={encodedToken}";
        var subject = "BioWings - Şifre Sıfırlama";
        var body = $@"
            <!DOCTYPE html>
            <html lang='tr'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>BioWings - Şifre Sıfırlama</title>
                <style>
                    body {{
                        font-family: 'Segoe UI', Arial, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 0;
                        background-color: #f4f4f4;
                    }}
                    .container {{
                        background-color: #ffffff;
                        margin: 20px auto;
                        padding: 0;
                        border-radius: 10px;
                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                        overflow: hidden;
                    }}
                    .header {{
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                        color: #ffffff;
                        padding: 40px 30px;
                        text-align: center;
                    }}
                    .header h1 {{
                        margin: 0;
                        font-size: 28px;
                        font-weight: 300;
                    }}
                    .header .logo {{
                        font-size: 20px;
                        font-weight: bold;
                        margin-bottom: 10px;
                    }}
                    .content {{
                        padding: 40px 30px;
                    }}
                    .greeting {{
                        font-size: 20px;
                        color: #2c3e50;
                        margin-bottom: 20px;
                    }}
                    .message {{
                        font-size: 16px;
                        margin-bottom: 30px;
                        color: #555;
                    }}
                    .button-container {{
                        text-align: center;
                        margin: 40px 0;
                    }}
                    .reset-button {{
                        display: inline-block;
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                        color: #ffffff;
                        padding: 15px 40px;
                        text-decoration: none;
                        border-radius: 30px;
                        font-size: 16px;
                        font-weight: 600;
                        box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
                        transition: all 0.3s ease;
                    }}
                    .reset-button:hover {{
                        transform: translateY(-2px);
                        box-shadow: 0 6px 20px rgba(102, 126, 234, 0.6);
                    }}
                    .info-box {{
                        background-color: #f8f9fa;
                        border-left: 4px solid #667eea;
                        padding: 20px;
                        margin: 30px 0;
                        border-radius: 5px;
                    }}
                    .info-box .icon {{
                        color: #667eea;
                        font-size: 18px;
                        margin-right: 10px;
                    }}
                    .footer {{
                        background-color: #f8f9fa;
                        padding: 30px;
                        text-align: center;
                        color: #6c757d;
                        font-size: 14px;
                    }}
                    .footer .social {{
                        margin-top: 20px;
                    }}
                    .footer .social a {{
                        color: #667eea;
                        text-decoration: none;
                        margin: 0 10px;
                    }}
                    .security-notice {{
                        background-color: #fff3cd;
                        border: 1px solid #ffeaa7;
                        color: #856404;
                        padding: 15px;
                        border-radius: 5px;
                        margin-top: 20px;
                        font-size: 14px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <div class='logo'>🦋 BioWings</div>
                        <h1>Şifre Sıfırlama Talebi</h1>
                    </div>
        
                    <div class='content'>
                        <div class='greeting'>
                            Merhaba {user.FirstName} {user.LastName},
                        </div>
            
                        <div class='message'>
                            Hesabınız için şifre sıfırlama talebinde bulundunuz. Yeni şifrenizi belirlemek için aşağıdaki butona tıklayarak güvenli sıfırlama sayfamıza yönlendirileceksiniz.
                        </div>
            
                        <div class='button-container'>
                            <a href='{url}' class='reset-button'>
                                🔐 Şifremi Sıfırla
                            </a>
                        </div>
            
                        <div class='info-box'>
                            <span class='icon'>⏰</span>
                            <strong>Önemli:</strong> Bu bağlantı güvenlik amacıyla <strong>1 saat</strong> sonra geçerliliğini yitirecektir.
                        </div>
            
                        <div class='security-notice'>
                            <strong>🛡️ Güvenlik Uyarısı:</strong> Eğer bu şifre sıfırlama talebinde bulunmadıysanız, bu e-postayı dikkate almayın ve hesabınızın güvenliğini kontrol edin.
                        </div>
                    </div>
        
                    <div class='footer'>
                        <p>Bu e-posta BioWings sistemi tarafından otomatik olarak gönderilmiştir.</p>
                        <p>Sorularınız için: <a href='mailto:support@biowings.com'>support@biowings.com</a></p>
            
                        <div class='social'>
                            <a href='#'>Gizlilik Politikası</a> | 
                            <a href='#'>Kullanım Şartları</a> | 
                            <a href='#'>Destek</a>
                        </div>
            
                        <p style='margin-top: 20px; font-size: 12px; color: #999;'>
                            © 2024 BioWings. Tüm hakları saklıdır.
                        </p>
                    </div>
                </div>
            </body>
            </html>";
        await emailService.SendEmailAsync(request.Email, subject, body);
        logger.LogInformation("Reset password link sent to {Email}", request.Email);
        return ServiceResult.Success();
    }
}
