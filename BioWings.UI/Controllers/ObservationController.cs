using BioWings.Application.Results;
using BioWings.UI.ViewModels.ObservationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using BioWings.Domain.Configuration;

namespace BioWings.UI.Controllers;

[Authorize]
public class ObservationController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ObservationController> _logger;
    private readonly string _baseUrl;

    public ObservationController(IHttpClientFactory httpClientFactory, ILogger<ObservationController> logger, IOptions<ApiSettings> apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _baseUrl = apiSettings.Value.BaseUrl;
    }

    public async Task<IActionResult> Index(string searchTerm, List<string> columnNames, List<string> columnValues, int pageNumber = 1, int pageSize = 25)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            //var token = User.FindFirst("AccessToken")?.Value;
            //if (!string.IsNullOrEmpty(token))
            //{
            //    client.DefaultRequestHeaders.Authorization =
            //        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            //}
            string url;

            // Çoklu filtreleme için kontrol
            if (columnNames != null && columnValues != null && columnNames.Count > 0 && columnNames.Count == columnValues.Count)
            {
                // URL için query parametreleri oluştur
                List<string> filterParams = new();

                for (int i = 0; i < columnNames.Count; i++)
                {
                    if (!string.IsNullOrEmpty(columnNames[i]) && !string.IsNullOrEmpty(columnValues[i]))
                    {
                        filterParams.Add($"columnNames={Uri.EscapeDataString(columnNames[i])}&columnValues={Uri.EscapeDataString(columnValues[i])}");
                    }
                }

                string filterQueryString = string.Join("&", filterParams);
                url = $"{_baseUrl}/Observations/Filter?{filterQueryString}&pageNumber={pageNumber}&pageSize={pageSize}";

                _logger.LogInformation($"Filtering observations with multiple filters: {filterQueryString}");
            }
            else if (!string.IsNullOrEmpty(searchTerm))
            {
                url = $"{_baseUrl}/Observations/Search?searchTerm={Uri.EscapeDataString(searchTerm)}&pageNumber={pageNumber}&pageSize={pageSize}";
                _logger.LogInformation($"Searching observations with term: {searchTerm}");
            }
            else
            {
                url = $"{_baseUrl}/Observations/Paged?pageNumber={pageNumber}&pageSize={pageSize}";
                _logger.LogInformation("Fetching all observations with paging");
            }

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

                // ViewBag'e filtreleme bilgilerini ekleme
                ViewBag.SearchTerm = searchTerm;
                ViewBag.ColumnNames = columnNames ?? new List<string>();
                ViewBag.ColumnValues = columnValues ?? new List<string>();

                return View(observations.Data);
            }

            _logger.LogError($"API request failed with status code: {response.StatusCode}");
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Error content: {errorContent}");
            ModelState.AddModelError(string.Empty, "An error occurred while fetching observations");
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in Index action");
            ModelState.AddModelError(string.Empty, $"An unexpected error occurred: {ex.Message}");
            return View();
        }
    }
}
