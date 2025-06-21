using BioWings.Application.Results;
using BioWings.Domain.Configuration;
using BioWings.UI.ViewModels.GenusViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;
[Authorize]
public class GenusController(IHttpClientFactory httpClientFactory, ILogger<GenusController> logger, IOptions<ApiSettings> options) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;
    public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 25)
    {
        if (pageSize <= 0) pageSize = 25;
        if (pageNumber <= 0) pageNumber = 1;
        var client = httpClientFactory.CreateClient("ApiClient");
        var url = string.IsNullOrEmpty(searchTerm)
            ? $"{_baseUrl}/Genera/Paged?pageNumber={pageNumber}&pageSize={pageSize}"
            : $"{_baseUrl}/Genera/Search?searchTerm={Uri.EscapeDataString(searchTerm)}&pageNumber={pageNumber}&pageSize={pageSize}";
        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ApiPaginatedResponse<GenusGetViewModel>>(content);
            logger.LogInformation("Genus fetched successfully");
            ViewBag.SearchTerm = searchTerm;
            return View(data.Data);
        }
        logger.LogError($"API request failed with status code: {response.StatusCode}");
        foreach (var error in ModelState.Values.SelectMany(x => x.Errors))
        {
            logger.LogError(error.ErrorMessage);
        }
        ModelState.AddModelError(string.Empty, "An error occurred while fetching genus");
        return View();
    }
}
