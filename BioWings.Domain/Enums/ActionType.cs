namespace BioWings.Domain.Enums;

/// <summary>
/// Aksiyon bazlı yetkilendirme için kullanılan yetki türlerini tanımlar
/// </summary>
public enum ActionType
{
    /// <summary>
    /// Okuma yetkisi
    /// </summary>
    Read = 1,

    /// <summary>
    /// Yazma/Ekleme yetkisi
    /// </summary>
    Write = 2,

    /// <summary>
    /// Güncelleme yetkisi
    /// </summary>
    Update = 3,

    /// <summary>
    /// Silme yetkisi
    /// </summary>
    Delete = 4
}
