using BioWings.Application.Features.Commands.UserRoleCommands;
using BioWings.Application.Features.Queries.UserRoleQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class UserRoleController(IMediator mediator) : BaseController
{
    [HttpGet("{userId}")]
    [AuthorizeDefinition("Kullanıcı-Rol Yönetimi", ActionType.Read, "Kullanıcı rollerini görüntüleme", AreaNames.Admin)]
    public async Task<IActionResult> GetUserRolesByUserId(int userId)
    {
        var query = new UserRoleGetByUserIdQuery(userId);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    [HttpPut("Range/{userId}")]
    [AuthorizeDefinition("Kullanıcı-Rol Yönetimi", ActionType.Update, "Kullanıcı rollerini güncelleme", AreaNames.Admin)]
    public async Task<IActionResult> UpdateRange(int userId, UserRoleUpdateRangeCommand userRoleUpdateRangeCommand)
    {
        userRoleUpdateRangeCommand.UserRoles.ForEach(x => x.UserId = userId);
        var result = await mediator.Send(userRoleUpdateRangeCommand);
        return CreateResult(result);
    }
}
