namespace BioWings.Application.DTOs.PermissionDTOs;

/// <summary>
/// Permission update işlemleri için ViewModel
/// </summary>
public class PermissionUpdateViewModel
{
    public int Id { get; set; }
    public string ControllerName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string Definition { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string HttpType { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
}
