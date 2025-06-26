namespace BioWings.Application.Features.Results.AuthorizeDefinitionResults;

/// <summary>
/// Yetkilendirme tanımı get query sonucu
/// </summary>
public class AuthorizeDefinitionGetQueryResult
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
    /// Menü adı
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// Action türü (Read, Write, Update, Delete)
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Açıklayıcı tanım
    /// </summary>
    public string Definition { get; set; } = string.Empty;

    /// <summary>
    /// Area adı
    /// </summary>
    public string AreaName { get; set; } = string.Empty;
} 