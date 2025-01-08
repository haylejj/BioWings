using Microsoft.AspNetCore.Mvc;

namespace BioWings.UI.Controllers;
public class ObserverController(IHttpClientFactory httpClientFactory,ILogger<ObserverController> logger ) : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
