using BioWings.Application.Results;
using BioWings.UI.ViewModels.ObserverViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;

[Authorize]
public class ObserverController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ObserverController> _logger;
    private readonly string _baseUrl;

    public ObserverController(IHttpClientFactory httpClientFactory, ILogger<ObserverController> logger, IOptions<BioWings.Domain.Configuration.ApiSettings> apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _baseUrl = apiSettings.Value.BaseUrl;
    }

    public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 25)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var url = string.IsNullOrEmpty(searchTerm)
            ? $"{_baseUrl}/Observers/Paged?pageNumber={pageNumber}&pageSize={pageSize}"
            : $"{_baseUrl}/Observers/Search?searchTerm={Uri.EscapeDataString(searchTerm)}&pageNumber={pageNumber}&pageSize={pageSize}";

        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var content = await response.Content.ReadAsStringAsync();
            var observations = JsonConvert.DeserializeObject<ApiPaginatedResponse<ObserverGetViewModel>>(content, settings);
            _logger.LogInformation("Observers fetched successfully");
            ViewBag.SearchTerm = searchTerm;
            return View(observations.Data);
        }
        _logger.LogError($"API request failed with status code: {response.StatusCode}");
        foreach (var error in ModelState.Values.SelectMany(x => x.Errors))
        {
            _logger.LogError(error.ErrorMessage);
        }
        ModelState.AddModelError(string.Empty, "An error occurred while fetching observers");
        return View();
    }

    [HttpGet]
    [Route("Observer/Observations/{id}")]
    public async Task<IActionResult> Observations(int id, string searchTerm, int pageNumber = 1, int pageSize = 25)
    {
        var client = _httpClientFactory.CreateClient("ApiClient"); // Use "ApiClient" for consistency
        var url = string.IsNullOrEmpty(searchTerm)
            ? $"{_baseUrl}/Observers/{id}/Observations?pageNumber={pageNumber}&pageSize={pageSize}"
            : $"{_baseUrl}/Observers/{id}/Observations/Search?searchTerm={Uri.EscapeDataString(searchTerm)}&pageNumber={pageNumber}&pageSize={pageSize}";

        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var observations = JsonConvert.DeserializeObject<ApiPaginatedResponse<ObserverGetObservationsViewModel>>(content);
            ViewBag.ObserverId = id;
            ViewBag.SearchTerm = searchTerm;
            return View(observations.Data);
        }

        _logger.LogError($"API request failed with status code: {response.StatusCode}");
        return View(new ApiPaginatedListResult<ObserverGetObservationsViewModel>());
    }
}
