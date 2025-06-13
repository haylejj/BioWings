using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.UI.Controllers;
[Authorize]
public class ObservationMapController(IHttpClientFactory httpClientFactory, ILogger<ObservationMapController> logger) : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
