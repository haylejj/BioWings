using BioWings.Application.DTOs.PermissionDTOs;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using BioWings.Domain.Enums;
using MediatR;
using System.Net;

namespace BioWings.Application.Features.Commands.PermissionCommands
{
    /// <summary>
    /// Permission create command handler
    /// </summary>
    public class PermissionCreateCommandHandler : IRequestHandler<PermissionCreateCommand, ServiceResult<PermissionGetViewModel>>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PermissionCreateCommandHandler(IPermissionRepository permissionRepository, IUnitOfWork unitOfWork)
        {
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<PermissionGetViewModel>> Handle(PermissionCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ActionType enum'a dönüştür
                if (!Enum.TryParse<ActionType>(request.ActionType, out var actionType))
                {
                    return ServiceResult<PermissionGetViewModel>.Error("Geçersiz ActionType değeri", HttpStatusCode.BadRequest);
                }

                // Permission oluştur
                var permission = new Permission(
                    request.ControllerName,
                    request.ActionName,
                    request.Definition,
                    actionType,
                    request.HttpType,
                    request.MenuName,
                    request.AreaName
                );

                // Aynı PermissionCode'a sahip permission var mı kontrol et
                var existingPermission = await _permissionRepository
                    .ExistsByPermissionCodeAsync(permission.PermissionCode, cancellationToken);

                if (existingPermission)
                {
                    return ServiceResult<PermissionGetViewModel>.Error(
                        $"Bu permission zaten mevcut: {permission.PermissionCode}", 
                        HttpStatusCode.Conflict);
                }

                // Veritabanına ekle
                await _permissionRepository.AddAsync(permission, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // ViewModel'e dönüştür
                var viewModel = new PermissionGetViewModel
                {
                    Id = permission.Id,
                    ControllerName = permission.ControllerName,
                    ActionName = permission.ActionName,
                    Definition = permission.Definition,
                    ActionType = permission.ActionType.ToString(),
                    HttpType = permission.HttpType,
                    MenuName = permission.MenuName,
                    AreaName = permission.AreaName,
                    PermissionCode = permission.PermissionCode,
                    CreatedDateTime = permission.CreatedDateTime,
                    UpdatedDateTime = permission.UpdatedDateTime
                };

                return ServiceResult<PermissionGetViewModel>.SuccessAsCreated(viewModel, $"/api/Permissions/{permission.Id}");
            }
            catch (Exception ex)
            {
                return ServiceResult<PermissionGetViewModel>.Error(
                    $"Permission oluşturulurken hata oluştu: {ex.Message}",
                    HttpStatusCode.InternalServerError);
            }
        }
    }
} 