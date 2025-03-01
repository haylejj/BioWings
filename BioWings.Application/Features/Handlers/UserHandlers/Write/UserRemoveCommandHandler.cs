using BioWings.Application.Features.Commands.UserCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.UserHandlers.Write;
public class UserRemoveCommandHandler(IUserRepository userRepository, ILogger<UserRemoveCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<UserRemoveCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(UserRemoveCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
            {
                logger.LogWarning("User removal failed: User with ID {UserId} not found", request.Id);
                return ServiceResult.Error("User not found", System.Net.HttpStatusCode.NotFound);
            }

            logger.LogInformation("Removing user: {UserId}, Email: {Email}, Name: {Name}",
                user.Id, user.Email, $"{user.FirstName} {user.LastName}");

            userRepository.Remove(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User successfully removed from database. ID: {UserId}, Email: {Email}",
                request.Id, user.Email);

            return ServiceResult.Success(System.Net.HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while removing user with ID: {UserId}", request.Id);
            return ServiceResult.Error("An error occurred while removing the user",
                System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
