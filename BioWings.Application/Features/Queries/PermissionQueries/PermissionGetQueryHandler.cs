using BioWings.Application.DTOs.PermissionDTOs;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using System.Net;

namespace BioWings.Application.Features.Queries.PermissionQueries;

/// <summary>
/// Permission get query handler
/// </summary>
public class PermissionGetQueryHandler : IRequestHandler<PermissionGetQuery, ServiceResult<List<PermissionGetViewModel>>>
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionGetQueryHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<ServiceResult<List<PermissionGetViewModel>>> Handle(PermissionGetQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var permissions = await _permissionRepository.GetAllAsync(cancellationToken);

            var viewModels = permissions.Select(p => new PermissionGetViewModel
            {
                Id = p.Id,
                ControllerName = p.ControllerName,
                ActionName = p.ActionName,
                Definition = p.Definition,
                ActionType = p.ActionType.ToString(),
                HttpType = p.HttpType,
                MenuName = p.MenuName,
                AreaName = p.AreaName,
                PermissionCode = p.PermissionCode,
                CreatedDateTime = p.CreatedDateTime,
                UpdatedDateTime = p.UpdatedDateTime
            }).ToList();

            return ServiceResult<List<PermissionGetViewModel>>.Success(viewModels);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<PermissionGetViewModel>>.Error(
                $"Permission'lar getirilirken hata oluştu: {ex.Message}",
                HttpStatusCode.InternalServerError);
        }
    }
}
