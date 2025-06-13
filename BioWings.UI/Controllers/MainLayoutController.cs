using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.UI.Controllers;
[AllowAnonymous]
public class MainLayoutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
