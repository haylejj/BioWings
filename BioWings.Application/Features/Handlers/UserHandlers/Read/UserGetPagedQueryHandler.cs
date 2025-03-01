using BioWings.Application.Features.Queries.UserQueries;
using BioWings.Application.Features.Results.UserResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.UserHandlers.Read;
public class UserGetPagedQueryHandler(IUserRepository userRepository, ILogger<UserGetPagedQueryHandler> logger) : IRequestHandler<UserGetPagedQuery, ServiceResult<PaginatedList<UserGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<UserGetPagedQueryResult>>> Handle(UserGetPagedQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Sayfa parametrelerini düzenleme
            request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);

            logger.LogInformation("Retrieving users with pagination. Page: {PageNumber}, Size: {PageSize}",
                request.PageNumber, request.PageSize);

            // Toplam kayıt sayısını alma
            var totalCount = await userRepository.GetTotalCountAsync(cancellationToken);
            logger.LogDebug("Total user count: {TotalCount}", totalCount);

            // Sayfalı veriyi getirme
            var users = await userRepository.GetPagedAsQueryable(request.PageNumber, request.PageSize)
                .ToListAsync(cancellationToken);

            var result = users.Select(x => new UserGetPagedQueryResult
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FullName = $"{x.FirstName} {x.LastName}".Trim(),
                Email = x.Email,
                IsEmailConfirmed = x.EmailConfirmed,
                CreatedTime = x.CreatedDateTime,
                UpdatedTime = x.UpdatedDateTime
            });

            var paginatedResult = new PaginatedList<UserGetPagedQueryResult>(
                result,
                totalCount,
                request.PageNumber,
                request.PageSize);

            logger.LogInformation(
                "Successfully retrieved {Count} users from page {PageNumber}. Total records: {TotalCount}",
                users.Count,
                request.PageNumber,
                totalCount);

            return ServiceResult<PaginatedList<UserGetPagedQueryResult>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error occurred while retrieving paged users. Page: {PageNumber}, Size: {PageSize}",
                request.PageNumber,
                request.PageSize);

            return ServiceResult<PaginatedList<UserGetPagedQueryResult>>.Error(
                "Failed to retrieve paged users");
        }
    }
}
