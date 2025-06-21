using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.UI.Controllers;
[Authorize]
public class ObservationMapController() : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
