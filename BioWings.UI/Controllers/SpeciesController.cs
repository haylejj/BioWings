using BioWings.Application.Results;
using BioWings.Domain.Configuration;
using BioWings.UI.ViewModels.SpeciesViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;
[Authorize]
public class SpeciesController(IHttpClientFactory httpClientFactory, ILogger<SpeciesController> logger, IOptions<ApiSettings> options) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;
    public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 25)
    {
        var client = httpClientFactory.CreateClient("ApiClient");
        var url = string.IsNullOrEmpty(searchTerm)
            ? $"{_baseUrl}/Species/Paged?pageNumber={pageNumber}&pageSize={pageSize}"
            : $"{_baseUrl}/Species/Search?searchTerm={Uri.EscapeDataString(searchTerm)}&pageNumber={pageNumber}&pageSize={pageSize}";
        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var species = JsonConvert.DeserializeObject<ApiPaginatedResponse<SpeciesGetViewModel>>(content);
            logger.LogInformation("Species fetched successfully");
            ViewBag.SearchTerm = searchTerm;
            return View(species.Data);
        }
        return View();
    }
}
