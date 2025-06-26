using BioWings.Application.DTOs.PermissionDTOs;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using BioWings.Domain.Enums;
using MediatR;
using System.Text.RegularExpressions;

namespace BioWings.Application.Features.Commands.PermissionCommands;

/// <summary>
/// Permission sync command handler
/// </summary>
public class PermissionSyncCommandHandler(
    IPermissionRepository permissionRepository,
    IRolePermissionRepository rolePermissionRepository,
    IAuthorizationDefinitionProvider authorizationDefinitionProvider,
    IUnitOfWork unitOfWork) : IRequestHandler<PermissionSyncCommand, ServiceResult<PermissionSyncResult>>
{
    public async Task<ServiceResult<PermissionSyncResult>> Handle(PermissionSyncCommand request, CancellationToken cancellationToken)
    {
        var authorizeDefinitions = authorizationDefinitionProvider.GetAuthorizeDefinitions();

        int addedCount = 0;
        int removedCount = 0;

        // 1. Kod'dan gelen permission code'larını oluştur
        var currentPermissionCodes = new HashSet<string>();
        var newPermissions = new List<Permission>();

        foreach (var definition in authorizeDefinitions)
        {
            var httpMethod = ExtractHttpMethod(definition.ActionName);
            var cleanActionName = CleanActionName(definition.ActionName);

            var areaName = string.IsNullOrWhiteSpace(definition.AreaName) ? "Global" : definition.AreaName;
            var permissionCode = $"{areaName}.{definition.ControllerName}.{cleanActionName}.{definition.ActionType}.{httpMethod}";
            
            currentPermissionCodes.Add(permissionCode);

            var exists = await permissionRepository.ExistsByPermissionCodeAsync(permissionCode, cancellationToken);

            if (!exists)
            {
                if (!Enum.TryParse<ActionType>(definition.ActionType, out var actionType))
                {
                    actionType = ActionType.Read;
                }

                var permission = new Permission(
                    controllerName: definition.ControllerName,
                    actionName: cleanActionName,
                    definition: definition.Definition,
                    actionType: actionType,
                    httpType: httpMethod,
                    menuName: definition.MenuName,
                    areaName: definition.AreaName
                );

                newPermissions.Add(permission);
                addedCount++;
            }
        }

        // 2. Yeni permission'ları ekle
        if (newPermissions.Any())
        {
            await permissionRepository.AddRangeAsync(newPermissions, cancellationToken);
        }

        // 3. Veritabanındaki tüm permission'ları al
        var allDbPermissions = await permissionRepository.GetAllAsync(cancellationToken);

        // 4. Kod'da artık olmayan permission'ları bul ve sil
        var permissionsToRemove = allDbPermissions
            .Where(p => !currentPermissionCodes.Contains(p.PermissionCode))
            .ToList();

        if (permissionsToRemove.Any())
        {
            // Önce bu permission'lara ait role-permission ilişkilerini sil
            foreach (var permission in permissionsToRemove)
            {
                await rolePermissionRepository.DeleteByPermissionIdAsync(permission.Id, cancellationToken);
            }

            // Sonra permission'ları sil
            permissionRepository.RemoveRange(permissionsToRemove);
            removedCount = permissionsToRemove.Count;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var result = new PermissionSyncResult
        {
            AddedCount = addedCount,
            RemovedCount = removedCount,
            TotalPermissions = currentPermissionCodes.Count
        };

        return ServiceResult<PermissionSyncResult>.Success(result);
    }

    /// <summary>
    /// Action adından HTTP method'unu çıkarır (örn: "Create [POST]" -> "POST")
    /// </summary>
    private string ExtractHttpMethod(string actionName)
    {
        var match = Regex.Match(actionName, @"\[([A-Z]+)\]");
        return match.Success ? match.Groups[1].Value : "GET";
    }

    /// <summary>
    /// Action adından HTTP method kısmını temizler (örn: "Create [POST]" -> "Create")
    /// </summary>
    private string CleanActionName(string actionName)
    {
        return Regex.Replace(actionName, @"\s*\[([A-Z]+)\]", "").Trim();
    }
}
