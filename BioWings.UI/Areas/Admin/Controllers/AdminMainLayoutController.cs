using Microsoft.AspNetCore.Mvc;

namespace BioWings.UI.Areas.Admin.Controllers;
public class AdminMainLayoutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
