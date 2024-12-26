using Microsoft.AspNetCore.Mvc;

namespace BioWings.UI.Controllers;
public class SpeciesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
