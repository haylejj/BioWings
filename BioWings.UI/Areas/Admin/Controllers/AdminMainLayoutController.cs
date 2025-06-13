using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.UI.Areas.Admin.Controllers;
[Authorize]
public class AdminMainLayoutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
