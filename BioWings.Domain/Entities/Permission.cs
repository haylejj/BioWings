using BioWings.Domain.Enums;

namespace BioWings.Domain.Entities
{
    /// <summary>
    /// Sistem yetkilendirme tanımlarını temsil eden entity
    /// </summary>
    public class Permission : BaseEntity
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Controller adı
        /// </summary>
        public string ControllerName { get; set; } = string.Empty;

        /// <summary>
        /// Action adı
        /// </summary>
        public string ActionName { get; set; } = string.Empty;

        /// <summary>
        /// Açıklayıcı tanım
        /// </summary>
        public string Definition { get; set; } = string.Empty;

        /// <summary>
        /// Yetki türü (Read, Write, Update, Delete)
        /// </summary>
        public ActionType ActionType { get; set; }

        /// <summary>
        /// HTTP method türü (GET, POST, PUT, DELETE vb.)
        /// </summary>
        public string HttpType { get; set; } = string.Empty;

        /// <summary>
        /// Menü adı
        /// </summary>
        public string MenuName { get; set; } = string.Empty;

        /// <summary>
        /// Area adı (boş olabilir)
        /// </summary>
        public string AreaName { get; set; } = string.Empty;

        /// <summary>
        /// Otomatik oluşturulan unique permission kodu
        /// Format: {AreaName}.{ControllerName}.{ActionName}.{ActionType}.{HttpType}
        /// </summary>
        public string PermissionCode { get; private set; } = string.Empty;

        /// <summary>
        /// Navigation property - RolePermissions
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        /// <summary>
        /// Permission code'unu otomatik oluşturur
        /// </summary>
        public void GeneratePermissionCode()
        {
            var areaName = string.IsNullOrWhiteSpace(AreaName) ? "Global" : AreaName;
            PermissionCode = $"{areaName}.{ControllerName}.{ActionName}.{ActionType}.{HttpType}";
        }

        /// <summary>
        /// Entity'nin tüm alanları ile birlikte oluşturulması
        /// </summary>
        public Permission(string controllerName, string actionName, string definition,ActionType actionType, string httpType, string menuName, string areaName = "")
       
        {
            ControllerName = controllerName;
            ActionName = actionName;
            Definition = definition;
            ActionType = actionType;
            HttpType = httpType;
            MenuName = menuName;
            AreaName = areaName;
            
            GeneratePermissionCode();
        }
    }
} 