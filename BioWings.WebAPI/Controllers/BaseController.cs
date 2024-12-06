using BioWings.Application.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BioWings.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    [NonAction]
    protected IActionResult CreateResult<T>(ServiceResult<T> result)
    {
        return result.StatusCode switch
        {
            HttpStatusCode.Created => Created(result.UrlAsCreated, result.Data),
            HttpStatusCode.NoContent => new ObjectResult(null) { StatusCode = (int)HttpStatusCode.NoContent },
            _ => new ObjectResult(result) { StatusCode= (int)result.StatusCode }
        };
    }
    [NonAction]
    protected IActionResult CreateResult(ServiceResult result)
    {
        return result.StatusCode switch
        {
            HttpStatusCode.NoContent => new ObjectResult(null) { StatusCode = (int)result.StatusCode },
            _ => new ObjectResult(result) { StatusCode = (int)result.StatusCode }
        };
    }
}
