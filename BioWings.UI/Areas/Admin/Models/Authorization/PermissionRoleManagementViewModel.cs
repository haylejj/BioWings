namespace BioWings.UI.Areas.Admin.Models.Authorization
{
    /// <summary>
    /// Permission-Role eşleşmeleri sayfası için ana ViewModel
    /// </summary>
    public class PermissionRoleManagementViewModel
    {
        /// <summary>
        /// Tüm permission'lar ve seçili rolleri
        /// </summary>
        public List<PermissionWithRolesViewModel> Permissions { get; set; } = new List<PermissionWithRolesViewModel>();

        /// <summary>
        /// Sistemdeki tüm roller
        /// </summary>
        public List<RoleInfo> Roles { get; set; } = new List<RoleInfo>();
    }
} 