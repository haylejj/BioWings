using BioWings.Application.DTOs;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.AuthorizeDefinitionQueries
{
    /// <summary>
    /// Tüm yetkilendirme tanımlarını getiren query
    /// </summary>
    public class AuthorizeDefinitionGetQuery : IRequest<ServiceResult<List<AuthorizeDefinitionViewModel>>>
    {
    }
} 