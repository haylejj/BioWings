using BioWings.Application.Results;
using BioWings.UI.ViewModels.StatisticsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;
[Authorize]
public class MainPageController(IHttpClientFactory httpClientFactory) : Controller
{
    public async Task<IActionResult> Index()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://localhost:7128/api/Statistics");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var jsonData = JsonConvert.DeserializeObject<ApiResponse<StatisticsGetViewModel>>(content);
            return View(jsonData.Data);
        }

        return View();
    }
}
