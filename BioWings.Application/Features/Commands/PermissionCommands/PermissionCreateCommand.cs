using BioWings.Application.DTOs.PermissionDTOs;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.PermissionCommands;

/// <summary>
/// Permission oluşturma command'ı
/// </summary>
public class PermissionCreateCommand : IRequest<ServiceResult<PermissionGetViewModel>>
{
    public string ControllerName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string Definition { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string HttpType { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
}
