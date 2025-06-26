using BioWings.Application.Features.Commands.LogoutCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.LogoutHandlers;

public class LogoutCommandHandler(
    ILogger<LogoutCommandHandler> logger, 
    IUserRepository userRepository, 
    IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("User logout attempt from IP: {IpAddress}, UserEmail: {UserEmail}", 
                request.IpAddress ?? "Unknown", 
                request.UserEmail ?? "Unknown");

            // Kullanıcı bilgilerini al
            var user = request.UserId.HasValue 
                ? await userRepository.GetByIdAsync(request.UserId.Value, cancellationToken)
                : !string.IsNullOrEmpty(request.UserEmail) 
                    ? await userRepository.GetByEmailAsync(request.UserEmail, cancellationToken)
                    : null;

            if (user != null)
            {
                // Refresh token'ı temizle
                user.RefreshToken = null;
                user.RefreshTokenExpireDate = null;
                userRepository.Update(user);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                logger.LogInformation("User {UserId} ({Email}) successfully logged out from IP: {IpAddress}", 
                    user.Id, user.Email, request.IpAddress ?? "Unknown");
            }
            else
            {
                logger.LogWarning("Logout attempt for unknown user: {UserEmail}", request.UserEmail ?? "Unknown");
            }
            
            return ServiceResult.Success(System.Net.HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during logout for user: {UserEmail}, IP: {IpAddress}", 
                request.UserEmail ?? "Unknown", request.IpAddress ?? "Unknown");
            return ServiceResult.Error("An error occurred during logout", System.Net.HttpStatusCode.InternalServerError);
        }
    }
} 