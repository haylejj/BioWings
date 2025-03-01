using BioWings.Application.Services;
using BioWings.Domain.Exceptions;
using BioWings.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
namespace BioWings.Infrastructure.Services;
public class EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(options.Value.SenderName, options.Value.SenderEmail));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
                bodyBuilder.HtmlBody = body;
            else
                bodyBuilder.TextBody = body;

            email.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(options.Value.SmtpServer, options.Value.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(options.Value.Username, options.Value.Password);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
            logger.LogInformation("Email sent successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email");
            throw new EmailServiceException("Failed to send email", ex);
        }
    }
}
