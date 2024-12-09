using BioWings.Application.Features.Queries.AuthorityQueries;
using BioWings.Application.Features.Results.AuthorityResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.AuthorityHandlers.Read;
public class AuthorityGetQueryHandler(IAuthorityRepository repository, ILogger<AuthorityGetQueryHandler> logger) : IRequestHandler<AuthorityGetQuery, ServiceResult<IEnumerable<AuthorityGetQueryResult>>>
{
    public async Task<ServiceResult<IEnumerable<AuthorityGetQueryResult>>> Handle(AuthorityGetQuery request, CancellationToken cancellationToken)
    {
        var authorities = await repository.GetAllAsync(cancellationToken);
        if (authorities==null || !authorities.Any())
        {
            logger.LogWarning("No authorities found");
            return ServiceResult<IEnumerable<AuthorityGetQueryResult>>.Error("No authorities found", System.Net.HttpStatusCode.NotFound);
        }
        var result = authorities.Select(a => new AuthorityGetQueryResult
        {
            Id = a.Id,
            Name = a.Name,
            Year = a.Year
        });
        logger.LogInformation("Authorities found");
        return ServiceResult<IEnumerable<AuthorityGetQueryResult>>.Success(result);
    }
}

