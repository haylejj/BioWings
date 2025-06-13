using BioWings.Application.Results;
using BioWings.UI.ViewModels.ObservationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;
[Authorize]
public class ObservationController(IHttpClientFactory httpClientFactory, ILogger<ObservationController> logger) : Controller
{
    public async Task<IActionResult> Index(string searchTerm, List<string> columnNames, List<string> columnValues, int pageNumber = 1, int pageSize = 25)
    {
        try
        {
            var client = httpClientFactory.CreateClient();
            string url;

            // Çoklu filtreleme için kontrol
            if (columnNames != null && columnValues != null && columnNames.Count > 0 && columnNames.Count == columnValues.Count)
            {
                // URL için query parametreleri oluştur
                List<string> filterParams = new List<string>();

                for (int i = 0; i < columnNames.Count; i++)
                {
                    if (!string.IsNullOrEmpty(columnNames[i]) && !string.IsNullOrEmpty(columnValues[i]))
                    {
                        filterParams.Add($"columnNames={Uri.EscapeDataString(columnNames[i])}&columnValues={Uri.EscapeDataString(columnValues[i])}");
                    }
                }

                string filterQueryString = string.Join("&", filterParams);
                url = $"https://localhost:7128/api/Observations/Filter?{filterQueryString}&pageNumber={pageNumber}&pageSize={pageSize}";

                logger.LogInformation($"Filtering observations with multiple filters: {filterQueryString}");
            }
            else if (!string.IsNullOrEmpty(searchTerm))
            {
                url = $"https://localhost:7128/api/Observations/Search?searchTerm={Uri.EscapeDataString(searchTerm)}&pageNumber={pageNumber}&pageSize={pageSize}";
                logger.LogInformation($"Searching observations with term: {searchTerm}");
            }
            else
            {
                url = $"https://localhost:7128/api/Observations/Paged?pageNumber={pageNumber}&pageSize={pageSize}";
                logger.LogInformation("Fetching all observations with paging");
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

            logger.LogError($"API request failed with status code: {response.StatusCode}");
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError($"Error content: {errorContent}");
            ModelState.AddModelError(string.Empty, "An error occurred while fetching observations");
            return View();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred in Index action");
            ModelState.AddModelError(string.Empty, $"An unexpected error occurred: {ex.Message}");
            return View();
        }
    }
}
