using BioWings.Application.Features.Commands.UserCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.UserHandlers.Write;
public class UserUpdateCommandHandler(IUserRepository userRepository, ILogger<UserUpdateCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<UserUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(UserUpdateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
            {
                logger.LogWarning("User update failed: User with ID {UserId} not found", request.Id);
                return ServiceResult.Error("User not found", System.Net.HttpStatusCode.NotFound);
            }

            // Email değişikliği olup olmadığını kontrol et
            if (user.Email != request.Email)
            {
                // Yeni email adresi kullanılıyor mu diye kontrol et
                var isEmailExists = await userRepository.IsExistByEmailAsync(request.Email, cancellationToken);
                if (!isEmailExists)
                {
                    logger.LogWarning("User update failed: Email {Email} already exists", request.Email);
                    return ServiceResult.Error("Email already exists", System.Net.HttpStatusCode.BadRequest);
                }
            }

            // Kullanıcı bilgilerini güncelleme
            logger.LogInformation("Updating user: {UserId}, Previous Email: {OldEmail}, New Email: {NewEmail}",
                user.Id, user.Email, request.Email);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.EmailConfirmed = request.IsEmailConfirmed;
            user.CountryId = request.CountryId;

            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User successfully updated. ID: {UserId}, Name: {Name}, Email: {Email}",
                user.Id, $"{user.FirstName} {user.LastName}", user.Email);

            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating user with ID: {UserId}", request.Id);
            return ServiceResult.Error("An error occurred while updating the user",
                System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
