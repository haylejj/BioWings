using BioWings.Application.Features.Queries.AuthorityQueries;
using BioWings.Application.Features.Results.AuthorityResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.AuthorityHandlers.Read;
public class AuthorityGetByIdQueryHandler(IAuthorityRepository repository, ILogger<AuthorityGetByIdQueryHandler> logger) : IRequestHandler<AuthorityGetByIdQuery, ServiceResult<AuthorityGetByIdQueryResult>>
{
    public async Task<ServiceResult<AuthorityGetByIdQueryResult>> Handle(AuthorityGetByIdQuery request, CancellationToken cancellationToken)
    {
        var authority = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (authority==null)
        {
            logger.LogWarning("Authority Id: {Id} not found", request.Id);
            return ServiceResult<AuthorityGetByIdQueryResult>.Error($"Authority Id: {request.Id} not found", System.Net.HttpStatusCode.NotFound);
        }
        var result = new AuthorityGetByIdQueryResult
        {
            Id = authority.Id,
            Name = authority.Name,
            Year = authority.Year
        };
        logger.LogInformation("Authority Id: {Id} found", request.Id);
        return ServiceResult<AuthorityGetByIdQueryResult>.Success(result);
    }
}
