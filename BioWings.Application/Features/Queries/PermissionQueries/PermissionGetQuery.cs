using BioWings.Application.DTOs.PermissionDTOs;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.PermissionQueries
{
    /// <summary>
    /// Tüm permission'ları getiren query
    /// </summary>
    public class PermissionGetQuery : IRequest<ServiceResult<List<PermissionGetViewModel>>>
    {
    }
} 