using BioWings.Application.Results;
using BioWings.UI.ViewModels.ObservationViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;
public class ObservationController(IHttpClientFactory httpClientFactory, ILogger<ObservationController> logger) : Controller
{
    public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 25)
    {
        var client = httpClientFactory.CreateClient();
        var url = string.IsNullOrEmpty(searchTerm)
            ? $"https://localhost:7128/api/Observations/Paged?pageNumber={pageNumber}&pageSize={pageSize}"
            : $"https://localhost:7128/api/Observations/Search?searchTerm={Uri.EscapeDataString(searchTerm)}&pageNumber={pageNumber}&pageSize={pageSize}";
        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var content = await response.Content.ReadAsStringAsync();
            var observations = JsonConvert.DeserializeObject<ApiPaginatedResponse<ObservationGetViewModel>>(content, settings);
            logger.LogInformation("Observations fetched successfully");
            ViewBag.SearchTerm = searchTerm;
            return View(observations.Data);
        }
        logger.LogError($"API request failed with status code: {response.StatusCode}");
        foreach (var error in ModelState.Values.SelectMany(x => x.Errors))
        {
            logger.LogError(error.ErrorMessage);
        }
        ModelState.AddModelError(string.Empty, "An error occurred while fetching observations");
        return View();
    }

}
