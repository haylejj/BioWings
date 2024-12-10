using Microsoft.AspNetCore.Mvc;

namespace BioWings.UI.Controllers;
public class MainLayoutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
