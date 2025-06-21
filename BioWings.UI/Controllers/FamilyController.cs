using BioWings.Application.Results;
using BioWings.Domain.Configuration;
using BioWings.UI.ViewModels.FamilyViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;
[Authorize]
public class FamilyController(IHttpClientFactory httpClientFactory, ILogger<FamilyController> logger, IOptions<ApiSettings> options) : Controller
{
    public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 25)
    {
        var baseUrl = options.Value.BaseUrl;
        var client = httpClientFactory.CreateClient("ApiClient");
        var url = string.IsNullOrEmpty(searchTerm)
            ? $"{baseUrl}/Families/Paged?pageNumber={pageNumber}&pageSize={pageSize}"
            : $"{baseUrl}/Families/Search?searchTerm={Uri.EscapeDataString(searchTerm)}&pageNumber={pageNumber}&pageSize={pageSize}";
        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {

            var content = await response.Content.ReadAsStringAsync();
            var families = JsonConvert.DeserializeObject<ApiPaginatedResponse<FamilyGetViewModel>>(content);
            logger.LogInformation("Families fetched successfully");
            ViewBag.SearchTerm = searchTerm;
            return View(families.Data);
        }
        logger.LogError($"API request failed with status code: {response.StatusCode}");
        foreach (var error in ModelState.Values.SelectMany(x => x.Errors))
        {
            logger.LogError(error.ErrorMessage);
        }
        ModelState.AddModelError(string.Empty, "An error occurred while fetching families");
        return View();
    }
}
