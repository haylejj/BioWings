using BioWings.Application.Results;
using BioWings.UI.ViewModels.FamilyViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;
[Authorize]
public class FamilyController(IHttpClientFactory httpClientFactory, ILogger<FamilyController> logger) : Controller
{
    public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 25)
    {
        var client = httpClientFactory.CreateClient();
        var url = string.IsNullOrEmpty(searchTerm)
        ? $"https://localhost:7128/api/Families/Paged?pageNumber={pageNumber}&pageSize={pageSize}"
            : $"https://localhost:7128/api/Families/Search?searchTerm={Uri.EscapeDataString(searchTerm)}&pageNumber={pageNumber}&pageSize={pageSize}";
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
