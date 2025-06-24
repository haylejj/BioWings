namespace BioWings.UI.Areas.Admin.Models.Authorization;

/// <summary>
/// Form submit için kullanılacak model
/// </summary>
public class SavePermissionRolesRequest
{
    /// <summary>
    /// Permission ID - Seçili Role ID'leri mapping'i
    /// </summary>
    public Dictionary<int, List<int>> PermissionRoles { get; set; } = new Dictionary<int, List<int>>();
}
