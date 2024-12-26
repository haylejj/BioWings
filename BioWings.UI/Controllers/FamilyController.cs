using Microsoft.AspNetCore.Mvc;

namespace BioWings.UI.Controllers;
public class FamilyController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
