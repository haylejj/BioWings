using BioWings.Application.Services;
using BioWings.Domain.Exceptions;
using BioWings.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace BioWings.Infrastructure.Services;

/// <summary>
/// MailHog için e-posta servisi - Development ortamında kullanılır
/// System.Net.Mail kullanır (MailKit yerine)
/// </summary>
public class MailHogService(IOptions<MailHogSettings> settings, ILogger<MailHogService> logger) : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            logger.LogInformation("Sending email to {Email} via MailHog", to);

            using SmtpClient smtpClient = new()
            {
                Host = settings.Value.Host,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Port = settings.Value.Port,
                EnableSsl = settings.Value.EnableSsl
            };

            MailMessage mailMessage = new()
            {
                From = new MailAddress(settings.Value.FromEmail, "BioWings Dev"),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);

            logger.LogInformation("Email sent to MailHog successfully. To: {To}, Subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to MailHog");
            throw new EmailServiceException("Failed to send email via MailHog", ex);
        }
    }
}
