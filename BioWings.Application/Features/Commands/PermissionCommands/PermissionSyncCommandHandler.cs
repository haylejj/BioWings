using MediatR;
using BioWings.Application.Results;
using BioWings.Application.Interfaces;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using BioWings.Domain.Enums;
using System.Text.RegularExpressions;

namespace BioWings.Application.Features.Commands.PermissionCommands
{
    /// <summary>
    /// Permission sync command handler
    /// </summary>
    public class PermissionSyncCommandHandler(
        IPermissionRepository permissionRepository,
        IAuthorizationDefinitionProvider authorizationDefinitionProvider,
        IUnitOfWork unitOfWork) : IRequestHandler<PermissionSyncCommand, ServiceResult<int>>
    {
        public async Task<ServiceResult<int>> Handle(PermissionSyncCommand request, CancellationToken cancellationToken)
        {
            var authorizeDefinitions = authorizationDefinitionProvider.GetAuthorizeDefinitions();
            
            int addedCount = 0;

            foreach (var definition in authorizeDefinitions)
            {

                var httpMethod = ExtractHttpMethod(definition.ActionName);
                var cleanActionName = CleanActionName(definition.ActionName);

                var areaName = string.IsNullOrWhiteSpace(definition.AreaName) ? "Global" : definition.AreaName;
                var permissionCode = $"{areaName}.{definition.ControllerName}.{cleanActionName}.{definition.ActionType}.{httpMethod}";

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

                    await permissionRepository.AddAsync(permission, cancellationToken);
                    addedCount++;
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<int>.Success(addedCount);
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
} 