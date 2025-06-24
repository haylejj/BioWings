namespace BioWings.UI.Areas.Admin.Models.Authorization;

/// <summary>
/// Role bilgisi
/// </summary>
public class RoleInfo
{
    /// <summary>
    /// Role ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Role adı
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
