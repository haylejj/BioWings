using BioWings.Application.Features.Queries.AuthorizeDefinitionQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers
{
    /// <summary>
    /// Yetkilendirme tanımlarını yöneten controller
    /// </summary>
    public class AuthorizeDefinitionsController(IMediator mediator) : BaseController
    {
        /// <summary>
        /// Tüm yetkilendirme tanımlarını getirir
        /// </summary>
        /// <returns>Yetkilendirme tanımlarının listesi</returns>
        [HttpGet]
        //[AuthorizeDefinition("Sistem Yönetimi", ActionType.Read, "Yetkilendirme tanımlarını görüntüleme", AreaNames.Admin)]
        public async Task<IActionResult> GetAll()
        {
            var query = new AuthorizeDefinitionGetQuery();
            var result = await mediator.Send(query);
            return CreateResult(result);
        }
    }
} 