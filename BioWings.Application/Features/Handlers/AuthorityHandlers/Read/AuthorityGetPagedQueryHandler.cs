using BioWings.Application.Features.Queries.AuthorityQueries;
using BioWings.Application.Features.Results.AuthorityResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.AuthorityHandlers.Read;
public class AuthorityGetPagedQueryHandler(IAuthorityRepository repository, ILogger<AuthorityGetPagedQueryHandler> logger) : IRequestHandler<AuthorityGetPagedQuery, ServiceResult<PaginatedList<AuthorityGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<AuthorityGetPagedQueryResult>>> Handle(AuthorityGetPagedQuery request, CancellationToken cancellationToken)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);
        var totalCount = await repository.GetTotalCountAsync(cancellationToken);

        var authorities = await repository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var result = authorities.Select(a => new AuthorityGetPagedQueryResult
        {
            Id = a.Id,
            Name = a.Name,
            Year = a.Year
        });
        var paginatedResult = new PaginatedList<AuthorityGetPagedQueryResult>(
            result,
            totalCount,
            request.PageNumber,
            request.PageSize);
        logger.LogInformation("Authorities found successfully with paged");
        return ServiceResult<PaginatedList<AuthorityGetPagedQueryResult>>.Success(paginatedResult);
    }
}
