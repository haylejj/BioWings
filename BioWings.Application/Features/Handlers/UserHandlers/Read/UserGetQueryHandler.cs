using BioWings.Application.Features.Queries.UserQueries;
using BioWings.Application.Features.Results.UserResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.UserHandlers.Read;
public class UserGetQueryHandler(IUserRepository userRepository, ILogger<UserGetQueryHandler> logger) : IRequestHandler<UserGetQuery, ServiceResult<List<UserGetQueryResult>>>
{
    public async Task<ServiceResult<List<UserGetQueryResult>>> Handle(UserGetQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await userRepository.GetAllAsNoTracking().Include(x => x.Country)
                .Select(u => new UserGetQueryResult
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = $"{u.FirstName} {u.LastName}".Trim(),
                    Email = u.Email,
                    CountryName = u.Country.Name,
                    IsEmailConfirmed = u.EmailConfirmed,
                    CreatedTime = u.CreatedDateTime,
                    UpdatedTime = u.UpdatedDateTime
                })
                .ToListAsync(cancellationToken);

            logger.LogInformation("Retrieved {Count} users successfully", users.Count);

            return ServiceResult<List<UserGetQueryResult>>.Success(users);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving users");
            return ServiceResult<List<UserGetQueryResult>>.Error("Failed to retrieve users");
        }
    }
}
