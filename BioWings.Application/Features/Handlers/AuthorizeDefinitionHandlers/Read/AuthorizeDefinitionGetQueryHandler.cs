using BioWings.Application.Features.Queries.AuthorizeDefinitionQueries;
using BioWings.Application.Features.Results.AuthorizeDefinitionResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using System.Net;

namespace BioWings.Application.Features.Handlers.AuthorizeDefinitionHandlers.Read;

/// <summary>
/// Yetkilendirme tanımlarını getiren query handler
/// </summary>
public class AuthorizeDefinitionGetQueryHandler : IRequestHandler<AuthorizeDefinitionGetQuery, ServiceResult<List<AuthorizeDefinitionGetQueryResult>>>
{
    private readonly IAuthorizationDefinitionProvider _authorizationDefinitionProvider;

    public AuthorizeDefinitionGetQueryHandler(IAuthorizationDefinitionProvider authorizationDefinitionProvider)
    {
        _authorizationDefinitionProvider = authorizationDefinitionProvider;
    }

    public async Task<ServiceResult<List<AuthorizeDefinitionGetQueryResult>>> Handle(AuthorizeDefinitionGetQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var definitions = _authorizationDefinitionProvider.GetAuthorizeDefinitions();
            
            var results = definitions.Select(d => new AuthorizeDefinitionGetQueryResult
            {
                ControllerName = d.ControllerName,
                ActionName = d.ActionName,
                MenuName = d.MenuName,
                ActionType = d.ActionType,
                Definition = d.Definition,
                AreaName = d.AreaName
            }).ToList();

            return ServiceResult<List<AuthorizeDefinitionGetQueryResult>>.Success(results, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<AuthorizeDefinitionGetQueryResult>>.Error(
                $"Yetkilendirme tanımları alınırken hata oluştu: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }
} 