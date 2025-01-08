using BioWings.Application.Results;
using BioWings.UI.ViewModels.ObserverViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;
public class ObserverController(IHttpClientFactory httpClientFactory, ILogger<ObserverController> logger) : Controller
{
    public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 25)
    {
        var client = httpClientFactory.CreateClient();
        var url = string.IsNullOrEmpty(searchTerm)
            ? $"https://localhost:7128/api/Observers/Paged?pageNumber={pageNumber}&pageSize={pageSize}"
            : $"https://localhost:7128/api/Observers/Search?searchTerm={Uri.EscapeDataString(searchTerm)}&pageNumber={pageNumber}&pageSize={pageSize}";
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
            logger.LogInformation("Observers fetched successfully");
            ViewBag.SearchTerm = searchTerm;
            return View(observations.Data);
        }
        logger.LogError($"API request failed with status code: {response.StatusCode}");
        foreach (var error in ModelState.Values.SelectMany(x => x.Errors))
        {
            logger.LogError(error.ErrorMessage);
        }
        ModelState.AddModelError(string.Empty, "An error occurred while fetching observers");
        return View();
    }
    [HttpGet]
    [Route("Observer/Observations/{id}")]
    public async Task<IActionResult> Observations(int id, string searchTerm, int pageNumber = 1, int pageSize = 25)
    {
        var client = httpClientFactory.CreateClient();
        var url = string.IsNullOrEmpty(searchTerm)
            ? $"https://localhost:7128/api/Observers/{id}/Observations?pageNumber={pageNumber}&pageSize={pageSize}"
            : $"https://localhost:7128/api/Observers/{id}/Observations/Search?searchTerm={Uri.EscapeDataString(searchTerm)}&pageNumber={pageNumber}&pageSize={pageSize}";

        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var observations = JsonConvert.DeserializeObject<ApiPaginatedResponse<ObserverGetObservationsViewModel>>(content);
            ViewBag.ObserverId = id;
            ViewBag.SearchTerm = searchTerm;
            return View(observations.Data);
        }

        logger.LogError($"API request failed with status code: {response.StatusCode}");
        return View(new ApiPaginatedListResult<ObserverGetObservationsViewModel>());
    }
}
