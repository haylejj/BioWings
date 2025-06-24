namespace BioWings.Application.DTOs.PermissionDTOs;

/// <summary>
/// Permission get işlemleri için ViewModel
/// </summary>
public class PermissionGetViewModel
{
    public int Id { get; set; }
    public string ControllerName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string Definition { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string HttpType { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty;
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
