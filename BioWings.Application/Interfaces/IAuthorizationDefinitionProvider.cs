using BioWings.Application.Features.Results.AuthorizeDefinitionResults;

namespace BioWings.Application.Interfaces;

/// <summary>
/// Yetkilendirme tanımlarını sağlayan servis interface'i
/// </summary>
public interface IAuthorizationDefinitionProvider
{
    /// <summary>
    /// Tüm AuthorizeDefinition attribute'larını tarar ve yetkilendirme tanımlarını döndürür
    /// </summary>
    /// <returns>Yetkilendirme tanımlarının listesi</returns>
    List<AuthorizeDefinitionGetQueryResult> GetAuthorizeDefinitions();
}
