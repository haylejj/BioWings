using BioWings.Application.Features.Queries.UserQueries;
using BioWings.Application.Features.Results.UserResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.UserHandlers.Read;
public class UserGetByIdQueryHandler(IUserRepository userRepository, ILogger<UserGetByIdQueryHandler> logger) : IRequestHandler<UserGetByIdQuery, ServiceResult<UserGetByIdQueryResult>>
{
    public async Task<ServiceResult<UserGetByIdQueryResult>> Handle(UserGetByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
            {
                logger.LogWarning("User with ID {UserId} was not found", request.Id);
                return ServiceResult<UserGetByIdQueryResult>.Error($"User with id {request.Id} not found", System.Net.HttpStatusCode.NotFound);
            }

            var result = new UserGetByIdQueryResult
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                CountryId = user.CountryId,
                IsEmailConfirmed = user.EmailConfirmed,
                CreatedTime = user.CreatedDateTime,
                UpdatedTime = user.UpdatedDateTime
            };

            logger.LogInformation("User with ID {UserId} was retrieved successfully", request.Id);

            return ServiceResult<UserGetByIdQueryResult>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving user with ID {UserId}", request.Id);
            return ServiceResult<UserGetByIdQueryResult>.Error($"Failed to retrieve user with id {request.Id}");
        }
    }
}
