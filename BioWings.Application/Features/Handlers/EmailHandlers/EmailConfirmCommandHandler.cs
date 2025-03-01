using BioWings.Application.Features.Commands.EmailCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.EmailHandlers;
public class EmailConfirmCommandHandler(IUserRepository userRepository, ILogger<EmailConfirmCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<EmailConfirmCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(EmailConfirmCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
            {
                logger.LogWarning("Email confirmation failed: User with email {Email} not found", request.Email);
                return ServiceResult.Error("Invalid confirmation link", System.Net.HttpStatusCode.BadRequest);
            }

            // Token kontrolü
            if (user.EmailConfirmationToken != request.Token)
            {
                logger.LogWarning("Email confirmation failed: Invalid token for user {Email}", request.Email);
                return ServiceResult.Error("Invalid confirmation token", System.Net.HttpStatusCode.BadRequest);
            }

            // Süre kontrolü
            if (user.EmailConfirmationTokenExpireDate < DateTime.UtcNow)
            {
                logger.LogWarning("Email confirmation failed: Token expired for user {Email}", request.Email);
                return ServiceResult.Error("Confirmation link has expired", System.Net.HttpStatusCode.BadRequest);
            }

            // Email doğrulama işlemini tamamla
            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpireDate = null;

            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Email confirmation successful for user {Email}", request.Email);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during email confirmation for {Email}", request.Email);
            return ServiceResult.Error("An error occurred during email confirmation",
                System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
