using BioWings.Application.Results;
using BioWings.UI.ViewModels.StatisticsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;

[Authorize]
public class MainPageController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    public MainPageController(IHttpClientFactory httpClientFactory, IOptions<BioWings.Domain.Configuration.ApiSettings> apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = apiSettings.Value.BaseUrl;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"{_baseUrl}/Statistics");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var jsonData = JsonConvert.DeserializeObject<ApiResponse<StatisticsGetViewModel>>(content);
            return View(jsonData.Data);
        }

        return View();
    }
}
