using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.UI.Areas.Admin.Controllers;
[AllowAnonymous]
public class AdminMainLayoutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
