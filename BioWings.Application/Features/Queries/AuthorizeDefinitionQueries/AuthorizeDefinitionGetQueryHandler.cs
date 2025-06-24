using BioWings.Application.DTOs;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using System.Net;

namespace BioWings.Application.Features.Queries.AuthorizeDefinitionQueries;

/// <summary>
/// Yetkilendirme tanımlarını getiren query handler
/// </summary>
public class AuthorizeDefinitionGetQueryHandler : IRequestHandler<AuthorizeDefinitionGetQuery, ServiceResult<List<AuthorizeDefinitionViewModel>>>
{
    private readonly IAuthorizationDefinitionProvider _authorizationDefinitionProvider;

    public AuthorizeDefinitionGetQueryHandler(IAuthorizationDefinitionProvider authorizationDefinitionProvider)
    {
        _authorizationDefinitionProvider = authorizationDefinitionProvider;
    }

    public async Task<ServiceResult<List<AuthorizeDefinitionViewModel>>> Handle(AuthorizeDefinitionGetQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var definitions = _authorizationDefinitionProvider.GetAuthorizeDefinitions();
            return ServiceResult<List<AuthorizeDefinitionViewModel>>.Success(definitions, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<AuthorizeDefinitionViewModel>>.Error(
                $"Yetkilendirme tanımları alınırken hata oluştu: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }
}
