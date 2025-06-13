namespace BioWings.UI.Areas.Admin.Models.Authorization
{
    /// <summary>
    /// Yetkilendirme tanımı görüntüleme model'i
    /// </summary>
    public class AuthorizationDefinitionViewModel
    {
        /// <summary>
        /// Controller adı
        /// </summary>
        public string ControllerName { get; set; } = string.Empty;

        /// <summary>
        /// Action adı (HTTP method ile birlikte)
        /// </summary>
        public string ActionName { get; set; } = string.Empty;

        /// <summary>
        /// Hangi menüye ait olduğu
        /// </summary>
        public string MenuName { get; set; } = string.Empty;

        /// <summary>
        /// Yetki türü (Read, Write, Update, Delete)
        /// </summary>
        public string ActionType { get; set; } = string.Empty;

        /// <summary>
        /// Açıklayıcı tanım
        /// </summary>
        public string Definition { get; set; } = string.Empty;

        /// <summary>
        /// Hangi Area'ya ait olduğu
        /// </summary>
        public string AreaName { get; set; } = string.Empty;
    }
} 