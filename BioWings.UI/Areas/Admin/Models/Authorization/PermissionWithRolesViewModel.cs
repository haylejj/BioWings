namespace BioWings.UI.Areas.Admin.Models.Authorization;

/// <summary>
/// Permission ve role eşleşmelerini gösteren ViewModel
/// </summary>
public class PermissionWithRolesViewModel
{
    /// <summary>
    /// Permission ID
    /// </summary>
    public int PermissionId { get; set; }

    /// <summary>
    /// Permission kodu
    /// </summary>
    public string PermissionCode { get; set; } = string.Empty;

    /// <summary>
    /// Controller adı
    /// </summary>
    public string ControllerName { get; set; } = string.Empty;

    /// <summary>
    /// Action adı
    /// </summary>
    public string ActionName { get; set; } = string.Empty;

    /// <summary>
    /// Açıklama
    /// </summary>
    public string Definition { get; set; } = string.Empty;

    /// <summary>
    /// Action türü
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// HTTP türü
    /// </summary>
    public string HttpType { get; set; } = string.Empty;

    /// <summary>
    /// Menü adı
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// Area adı
    /// </summary>
    public string AreaName { get; set; } = string.Empty;

    /// <summary>
    /// Bu permission için seçili rol ID'leri
    /// </summary>
    public List<int> SelectedRoleIds { get; set; } = new List<int>();
}
