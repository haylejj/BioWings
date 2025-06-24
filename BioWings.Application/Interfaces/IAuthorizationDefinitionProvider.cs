using BioWings.Application.DTOs;

namespace BioWings.Application.Interfaces;

/// <summary>
/// Yetkilendirme tanımlarını sağlayan servisin arayüzü
/// </summary>
public interface IAuthorizationDefinitionProvider
{
    /// <summary>
    /// Tüm AuthorizeDefinition attribute'larını tarar ve yetkilendirme tanımlarını döndürür
    /// </summary>
    /// <returns>Yetkilendirme tanımlarının listesi</returns>
    List<AuthorizeDefinitionViewModel> GetAuthorizeDefinitions();
}
