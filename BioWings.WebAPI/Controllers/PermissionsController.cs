using BioWings.Application.Features.Commands.PermissionCommands;
using BioWings.Application.Features.Queries.PermissionQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;

/// <summary>
/// Permission yönetimi controller'ı
/// </summary>
public class PermissionsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Tüm permission'ları getirir
    /// </summary>
    /// <returns>Permission listesi</returns>
    [HttpGet]
    [AuthorizeDefinition("Yetki Yönetimi", ActionType.Read, "Tüm yetkileri görüntüleme", AreaNames.Admin)]
    public async Task<IActionResult> GetAll()
    {
        var query = new PermissionGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    /// <summary>
    /// Yeni permission oluşturur
    /// </summary>
    /// <param name="command">Permission create command</param>
    /// <returns>Oluşturulan permission</returns>
    [HttpPost]
    [AuthorizeDefinition("Yetki Yönetimi", ActionType.Write, "Yeni yetki oluşturma", AreaNames.Admin)]
    public async Task<IActionResult> Create([FromBody] PermissionCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    /// <summary>
    /// Yetkilendirme tanımlarını veritabanıyla senkronize eder
    /// </summary>
    /// <returns>Eklenen yetki sayısı</returns>
    [HttpPost("sync")]
    [AuthorizeDefinition("Yetki Yönetimi", ActionType.Write, "Yetkileri senkronize etme", AreaNames.Admin)]
    public async Task<IActionResult> Sync()
    {
        var command = new PermissionSyncCommand();
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
