namespace BioWings.Domain.Entities;

/// <summary>
/// Role ve Permission arasındaki many-to-many ilişkiyi temsil eden entity
/// </summary>
public class RolePermission
{
    /// <summary>
    /// Role ID (Composite Primary Key)
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Permission ID (Composite Primary Key)
    /// </summary>
    public int PermissionId { get; set; }

    /// <summary>
    /// Navigation property - Role
    /// </summary>
    public virtual Role Role { get; set; }

    /// <summary>
    /// Navigation property - Permission
    /// </summary>
    public virtual Permission Permission { get; set; }
}
