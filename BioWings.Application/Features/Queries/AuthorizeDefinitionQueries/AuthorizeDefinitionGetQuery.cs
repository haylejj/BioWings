using BioWings.Application.Features.Results.AuthorizeDefinitionResults;
using BioWings.Application.Results;
using MediatR;
using System.Net;

namespace BioWings.Application.Features.Queries.AuthorizeDefinitionQueries;

/// <summary>
/// Tüm yetkilendirme tanımlarını getiren query
/// </summary>
public class AuthorizeDefinitionGetQuery : IRequest<ServiceResult<List<AuthorizeDefinitionGetQueryResult>>>
{
}


